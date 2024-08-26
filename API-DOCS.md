All endpoinst have that type of routing  `{URL}/api/{CONTROLLER_NAME}/{NANE}`
So here will be written onnly `{CONTROLLER_NAME}/{NANE}` part
### 1. Authorization:
 1. POST `Auth/create` </br>
    Accept JSON in body</br>
```json
  {
    "email": "string@gmail.com",
    "password": "12345678",
    "firstName": "String",
    "lastName": "Intovich"
  }
```
 2. POST `Auth/login`</br>
    Accept JSON in body</br>
```json
   {
    "email": "string",
    "password": "string"
   }
```
   Set tokens in coockies and return token in headers
```json
  "refresh-token": "rS6Ke7WRbC/n7AlvYgFRagsbva1gYIdVzNy3x2K62/rcX6oMpEZj5z6sfxYo/xcbfcptyJV1gg=="
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJlM2U4ZDFlNS1lMzFlLTQ4YWEtMTQzYS0wOGRjOTYyZjJkMjIiLCJodHRwOi8vc2NoZW1hcy5taWNyb3N"
```

 3. POST `Auth/refresh-tokens`</br>
   Accept refresh and JWT tokens in header</br>
   Set new tokens in coockies and return token in headers
```json
 "refresh-token": "rS6Ke7WRbC/n7AlvYgFRagsbva1gYIdVzNy3x2K62/rcX6oMpEZj5z6sfxYo/xcbfcptyJV1gg=="
"token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJlM2U4ZDFlNS1lMzFlLTQ4YWEtMTQzYS0wOGRjOTYyZjJkMjIiLCJodHRwOi8vc2NoZW1hcy5taWNyb3N"
```
### ❗❗❗ ALL NEXT ENDPOINT NEED JWT TOKEN TO WORK ❗❗❗
### 2. UserDb

