using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Kadmium_Dmx_Processor.Actors;
using Kadmium_Dmx_Processor.Effects.FixtureEffects.LightFixtureEffects;
using Kadmium_Dmx_Shared.Models;
using Kadmium_Dmx_Processor.Services.Groups;
using Kadmium_Dmx_Processor.Services.Venues;

namespace Kadmium_Dmx_Processor.Services.Mqtt
{
	public class MqttEventHandler : IMqttEventHandler
	{
		private IGroupProvider GroupProvider { get; }
		private IVenueProvider VenueProvider { get; }
		private IMqttProvider MqttProvider { get; }

		public MqttEventHandler(IGroupProvider groupProvider, IVenueProvider venueProvider, IMqttProvider mqttProvider)
		{
			GroupProvider = groupProvider;
			VenueProvider = venueProvider;
			MqttProvider = mqttProvider;
		}

		public async Task Handle(MqttEvent mqttEvent)
		{
			var topicParts = mqttEvent.Topic.Split("/", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

			switch (topicParts[0])
			{
				case "group":
					await HandleGroupEvent(topicParts, mqttEvent.Payload);
					break;
				case "fixture":
					HandleFixtureEvent(topicParts, mqttEvent.Payload);
					break;
				case "venue":
					HandleVenueEvent(topicParts, mqttEvent.Payload);
					break;
			}
		}

		private async Task HandleGroupEvent(string[] topicParts, byte[] payload)
		{
			var parsed = System.Text.Encoding.ASCII.GetString(payload[2..]);
			var value = Single.Parse(parsed);

			var groupName = topicParts[1];
			if (GroupProvider.Groups.ContainsKey(groupName))
			{
				var group = GroupProvider.Groups[groupName];
				var attributeName = topicParts[2];
				if (group.EffectAttributes.ContainsKey(attributeName))
				{
					group.EffectAttributes[attributeName].Value = value;
				}
				else
				{
					var messages = group.Fixtures.Keys.Select(async (x) =>
					{
						var topic = $"/fixture/{x.UniverseId}/{x.Address}/{attributeName}";
						await MqttProvider.Send(topic, payload);
					});
					await Task.WhenAll(messages);
				}
			}
		}

		private void HandleFixtureEvent(string[] topicParts, byte[] payload)
		{
			var parsed = System.Text.Encoding.ASCII.GetString(payload[2..]);
			var value = Single.Parse(parsed);

			var parsedUniverse = ushort.TryParse(topicParts[1], out ushort universeId);
			if (parsedUniverse && VenueProvider.UniverseActors.ContainsKey(universeId))
			{
				var universe = VenueProvider.UniverseActors[universeId];
				var parsedAddress = ushort.TryParse(topicParts[2], out ushort fixtureAddress);
				if (parsedAddress && universe.FixtureActors.ContainsKey(fixtureAddress))
				{
					var fixture = universe.FixtureActors[fixtureAddress];
					HandleFixtureSet(new[] { fixture }, topicParts.Last(), value);
				}
			}
		}

		private void HandleFixtureSet(IEnumerable<FixtureActor> fixtures, string attribute, Single value)
		{
			if (attribute == LightFixtureConstants.Hue)
			{
				value *= 360f;
			}
			foreach (var fixture in fixtures)
			{
				fixture.EffectAttributes[attribute].Value = value;
			}
		}

		private void HandleVenueEvent(string[] topicParts, byte[] payload)
		{
			switch (topicParts[1])
			{
				case "load":
					var document = JsonDocument.Parse(payload);
					VenueProvider.LoadVenue(document);
					break;
			}
		}
	}
}