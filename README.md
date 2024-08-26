<h1>InSightWindow Application</h1>

InSightWindow is designed to solve the problem of difficult window access. Our app allows you to operate your windows remotely. You can easily monitor parameters, control the opening and closing, and ensure the security of your windows—all from your device.

This is an API part which allow you to log/sign in and grab/send data to window

⬇️<strong>Installiation</strong>⬇️
1. Pull this repo
2. Install docker
3. Build docker container 
4. Run docker compose 
<strong>OR</strong>
1. Go to that url
2. Pull all docker images
3 Run them using docker-compose file (locate here).

📌The difference that when you downlod it from github you will have and avalibility to source code.📌


📑<strong>Here is some API docs for developers</strong>.📑/<br>
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




