# Passkey Blazor App

**Author:** Jan Franěk

A Blazor web application implementing secure, passwordless authentication using FIDO2 WebAuthn. This app demonstrates modern authentication mechanisms, enhancing security and user experience.

You can try the live demo [here](https://franekj.bridge.cz/).
You can find the full specification (in Czech) [here](SPECIFICATION.md).

---

## **Table of Contents**

- [Features](#features)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Configuration](#configuration)
- [Running the Application](#running-the-application)
- [Usage](#usage)
- [Troubleshooting](#troubleshooting)
- [Acknowledgments](#acknowledgments)

---

## **Features**

- Passwordless authentication with FIDO2 WebAuthn.
- Secure communication over HTTPS.
- User registration and login functionality.
- SQLite database integration for data storage.
- Cross-platform support.

---

## **Prerequisites**

Ensure you have the following installed:

- [.NET SDK 9.0 or later](https://dotnet.microsoft.com/download)
- [Nginx](https://www.nginx.com/) (for deployment)

The project has been tested with most of the conventional web browsers, including Chrome, Firefox, Edge and LibeWolf
 and with many authenticators, including Bitwarden browser extension, Windows Hello, Google Password Manager.

---

## **Installation**

### **1. Clone the Repository**

```bash
git clone https://github.com/jan-franek/PasskeyBlazorApp.git
cd PasskeyBlazorApp
```

### **2. Restore Dependencies**

```bash
dotnet restore
```

### **3. Build the Project**

```bash
dotnet build BlazorApp.csproj -c Release -o app/build
```

### ***4. Publish the Project**

```bash
dotnet publish "./BlazorApp.csproj" -c Release -o app/publish /p:UseAppHost=false
```

---

## **Configuration**

### **App Settings**

Edit the `appsettings.json` file to configure your application's settings:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Fido2": {
    "ServerDomain": "yourdomain.com",
    "Origins": "https://yourdomain.com"
  }
}
```

- **ServerDomain**: The domain where your app is hosted.
- **Origins**: Allowed origins for FIDO2 authentication.

### **HTTPS Configuration**

Ensure that your application is set up to use HTTPS:

- Generate a self-signed certificate or use a trusted certificate.
- Update your `Program.cs` to configure Kestrel for HTTPS.

---

## **Running the Application**

You can run the application locally using the .NET CLI

```bash
dotnet run
```

---

## **Deployment**

I've deployed the live app using `Nginx`. The `Dockerfile` is currently non-functional.

## **Usage**

### **Accessing the Application**

- **Web Interface**: Navigate to `https://yourdomain.com` in your web browser.

### **User Registration**

- Go to `/register` to create a new account using FIDO2 authentication.

### **User Login**

- Go to `/login` to authenticate with your registered credentials.

---

## **Troubleshooting**

### **Common Issues**

#### **1. HTTPS Errors**

- Ensure that your SSL certificates are correctly installed and configured.
- Verify that the application URLs match the ones specified in `appsettings.json`.

#### **2. Database Permissions**

- Make sure the `app.db` file has the correct permissions:

  ```bash
  sudo chown www-data:www-data app.db
  sudo chmod 664 app.db
  ```

- Replace `www-data` with the user running your application.

#### **3. Origin Mismatch Errors**

- Ensure that the `Fido2:Origins` in `appsettings.json` match the domain and ports your app is running on.

#### **4. Port Conflicts**

- Confirm that the ports your application is using are not occupied by other services.

## **Acknowledgments**

- **FIDO2 NetLib**: [GitHub Repository](https://github.com/passwordless-lib/fido2-net-lib)
- **Microsoft Blazor**: [Official Documentation](https://docs.microsoft.com/aspnet/core/blazor)
- **SQLite**: [Official Website](https://www.sqlite.org/index.html)
- **Nginx**: [Official Website](https://www.nginx.com/)
