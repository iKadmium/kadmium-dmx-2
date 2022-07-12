# Introduction
sAcnRenderer will take DMX from the MQTT queue under the /universe/{universeId} topic and output them to sAcn. It supports unicast and multicast over IPv4 and IPv6.

# Configuration
Configuration is through environment variables.
* IPV4_MULTICAST (boolean, default true) => enables IPV4 multicast
* IPV6_MULTICAST (boolean, default false) => enables IPV6 multicast
* UNICAST_TARGETS (string, default empty) => a comma-separated list of unicast targets. If empty, will not send any unicast packets
* MQTT_HOST (string, default "mqtt") => mqtt host to connect to