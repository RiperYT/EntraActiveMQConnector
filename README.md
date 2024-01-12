# EntraActiveMQConnector
Application settings in EntraID:
1. Application -> API permissions -> Add a permission -> Microsoft Graph (need to choose "Application permissions" and than turn on "Directory.Read.All" permissions). After it press "Grant admin consent for <name-of-directory>"
2. Application -> Authentication -> Add a platform -> Mobile and desktop (turn on "https://login.microsoftonline.com/common/oauth2/nativeclient" and paste URI of application, for example: "https://localhost:7137/")
3. Application -> Authentication -> Advanced settings (turn on "Enable the following mobile and desktop flows:")

Instructions for using service:
1. Start the service
2. Open swagger(if you want) on browswer: <service-url>/swagger
3. Use /Settings/setEntraId contoller to config entra connection. You need to send the secure token for your app, tenantId, clientId(you can find it in Application -> Overview) and client secret(Application -> Certificates & secrets -> Client secrets -> New client secret and than copy a value of new secret)
4. Use /Settings/setActiveMQ contoller to config artemis. You need to send the secure token for your app, domen, port, login, password and queueName.

To do opperations with controller you need to use a secure token(code). Standart - 123, but you can edit it in appsettings.json
