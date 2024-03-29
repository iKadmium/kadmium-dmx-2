using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kadmium_Dmx_Processor.Effects;
using Kadmium_Dmx_Processor.Effects.FixtureEffects.LightFixtureEffects;
using Kadmium_Dmx_Processor.Services.TimeProvider;
using Moq;

namespace Kadmium_Dmx_Processor.Test.Effects.FixtureEffects.LightFixtureEffects
{
	public class FakeStrobeTests
	{
		[Fact]
		public void Given_StrobeIsZero_When_ApplyIsCalled_Then_BrightnessIsNotAffected()
		{
			var expected = 1.0f;
			var timeProviderMock = Mock.Of<ITimeProvider>();
			var fixture = FixtureHelper.GetFixture(
				LightFixtureConstants.Red,
				LightFixtureConstants.Green,
				LightFixtureConstants.Blue
			);
			var pipeline = fixture.FramePipeline;
			var fakeStrobe = new FakeStrobe(timeProviderMock, fixture);

			pipeline[LightFixtureConstants.Shutter].Value = 0.0f;
			pipeline.Add(LightFixtureConstants.Brightness, new EffectAttribute { Value = expected });

			fakeStrobe.Apply(pipeline);
			var actual = pipeline[LightFixtureConstants.Brightness];

			Assert.Equal(expected, actual.Value);
		}

		[Fact]
		public void Given_StrobeIs1_And_TimeHasNotPassed_When_ApplyIsCalled_Then_BrightnessIsNotAffected()
		{
			var expected = 1.0f;
			var timeProviderMock = Mock.Of<ITimeProvider>(mock =>
				mock.TimeSinceLastRender == TimeSpan.FromMilliseconds(0)
			);
			var fixture = FixtureHelper.GetFixture(
				LightFixtureConstants.Red,
				LightFixtureConstants.Green,
				LightFixtureConstants.Blue
			);
			var pipeline = fixture.FramePipeline;
			var fakeStrobe = new FakeStrobe(timeProviderMock, fixture);

			pipeline[LightFixtureConstants.Shutter].Value = 1.0f;
			pipeline.Add(LightFixtureConstants.Brightness, new EffectAttribute { Value = 1.0f });

			fakeStrobe.Apply(pipeline);
			var actual = pipeline[LightFixtureConstants.Brightness];

			Assert.Equal(expected, actual.Value);
		}

		[Fact]
		public void Given_StrobeIs1_And_TimeHasPassed_When_ApplyIsCalled_Then_BrightnessIsZeroed()
		{
			var expected = 0.0f;
			var timeProviderMock = Mock.Of<ITimeProvider>(mock =>
				mock.TimeSinceLastRender == TimeSpan.FromMilliseconds(FakeStrobe.FASTEST_CYCLE_TIME_MS + 1)
			);
			var fixture = FixtureHelper.GetFixture(
				LightFixtureConstants.Red,
				LightFixtureConstants.Green,
				LightFixtureConstants.Blue
			);
			var pipeline = fixture.FramePipeline;
			var fakeStrobe = new FakeStrobe(timeProviderMock, fixture);

			pipeline[LightFixtureConstants.Shutter].Value = 1.0f;
			pipeline.Add(LightFixtureConstants.Brightness, new EffectAttribute { Value = 1.0f });

			fakeStrobe.Apply(pipeline);
			var actual = pipeline[LightFixtureConstants.Brightness];

			Assert.Equal(expected, actual.Value);
		}

		[Fact]
		public void Given_StrobeIs1_And_TimeHasPassedAgain_When_ApplyIsCalled_Then_BrightnessIsZeroed()
		{
			var expected = 1.0f;
			var timeProviderMock = Mock.Of<ITimeProvider>(mock =>
				mock.TimeSinceLastRender == TimeSpan.FromMilliseconds(FakeStrobe.FASTEST_CYCLE_TIME_MS + 1)
			);
			var fixture = FixtureHelper.GetFixture(
				LightFixtureConstants.Red,
				LightFixtureConstants.Green,
				LightFixtureConstants.Blue
			);
			var pipeline = fixture.FramePipeline;
			var fakeStrobe = new FakeStrobe(timeProviderMock, fixture);

			pipeline[LightFixtureConstants.Shutter].Value = 1.0f;
			pipeline.Add(LightFixtureConstants.Brightness, new EffectAttribute { Value = 1.0f });

			fakeStrobe.Apply(pipeline);
			pipeline[LightFixtureConstants.Brightness].Value = 1.0f;
			fakeStrobe.Apply(pipeline);
			var actual = pipeline[LightFixtureConstants.Brightness];

			Assert.Equal(expected, actual.Value);
		}
	}
}