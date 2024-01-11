# EntraActiveMQConnector
Application settings in EntraID:
1. Application -> API permissions -> Add a permission -> Microsoft Graph (need to turn on "profile" and "User.Read.All" permissions). After it press "Grant admin consent for <name-of-directory>"
2. Application -> Authentication -> Add a platform -> Mobile and desktop (turn on "https://login.microsoftonline.com/common/oauth2/nativeclient" and paste URI of application, for example: "https://localhost:7137/")
3. Application -> Authentication -> Advanced settings (turn on "Enable the following mobile and desktop flows:")

Instructions for using service:
1. Start the service
2. Open swagger(if you want) on browswer: <service-url>/swagger
3. Use /Settings/setEntraId contoller to config entra connection. You need to send the secure token for your app, tenantId and clientId(you can find it in Application -> Overview), after it you will get link and code. Go to this link and paste code
4. Use /Settings/confirmEntraId contoller after entering code.
5. Use /Settings/setActiveMQ contoller to config artemis. You need to send the secure token for your app, domen, port, login, password and queueName.
