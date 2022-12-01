## 0.1.9 (01-12-2022):

Add options to specify the way to resolve hostname to ip-addresses

## 0.1.8 (28-04-2022):

Do not resolve and cache string ip addresses.

## 0.1.7 (06-12-2021):

Added `net6.0` target.

## 0.1.5 (06-08-2021)

Increase GetDatacenter* methods performance.

## 0.1.4 (06-04-2020):

Local datacenter can be configured using settings or environment variables.

## 0.1.2 (18-02-2020):

- DnsResolver: deduplicate initial resolve calls.
- Datacenters: added nonblocking GetDatacenterWeak method that does not wait for DNS resolution.

## 0.1.1 (05-11-2019):

Added static `DatacentersProvider`, `EmptyDatacenters` implementation, `LocalDatacenterIsActive` extension.

## 0.1.0 (27-09-2019): 

Initial prerelease.