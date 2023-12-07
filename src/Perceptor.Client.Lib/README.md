## Working locally

### Build

```bash
dotnet build Perceptor.Client.Lib.Net.sln
```

### Run unit tests

```bash
dotnet test Perceptor.Client.Lib.Net.sln
```

### Integration tests

Integration tests are to be run from IDE only and not automatically. That's why they are per default attributed as
_Ignore_.
The _Ignore_ attribute has to be commented out in order to run the tests.</br>

Configuration has to be specified for the test. Create a file named "client-config.json" in the root folder of
_Perceptor.Client.Lib.Test_. The file has to have following content:

```json
{
  "TAI_PERCEPTOR_API_KEY":"",
  "TAI_PERCEPTOR_BASE_URL":""
}
```

Add respective values for _api_key_ and _base_url_.

