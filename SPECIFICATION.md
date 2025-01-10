# **Specifikace aplikace**

## **1. Úvod**

### **1.1 Cíl projektu**

Cílem projektu je vytvořit webovou aplikaci, která umožňuje uživatelům registraci a přihlášení bez použití hesel. Pro autentifikaci je implementován standard **FIDO2 WebAuthn**, který nabízí bezpečný způsob ověřování identity.

Aplikaci v běhu můžete vidět [zde](https://franekj.bridge.cz/).

---

## **2. Funkcionalita aplikace**

### **2.1 Hlavní funkce**

- **Registrace uživatele:**
  Uživatel zadá své uživatelské jméno a pomocí standardu FIDO2 si vytvoří `passkey` na svém zařízení.

- **Přihlášení uživatele:**
  Uživatel ověří svou identitu použitím již vytvořeného `passkey`.

- **Odhlášení uživatele:**
  Uživatel se může kdykoliv bezpečně odhlásit.

### **2.2 Bezpečnost**

- Podpora HTTPS pro bezpečnou komunikaci mezi klientem a serverem.
- Využití standardu FIDO2 pro eliminaci rizik spojených s únikem hesel.

---

## **3. Technologie**

### **3.1 Backend**

- **Platforma:** .NET 9 (ASP.NET Core Blazor Server)
- **Databáze:** SQLite
- **Autentifikační knihovna:** Fido2NetLib

### **3.2 Frontend**

- **Framework:** Blazor
- **WebAuthn API:** Využíváno pro komunikaci mezi klientem a autentifikačním zařízením uživatele.

### **3.3 Deployment**

- Webový server: **Nginx**

---

## **4. Autentifikace**

### **4.1 Registrace**

Proces registrace slouží k vytvoření nového passkey. Postup je následující:

1. Uživatel zadá své uživatelské jméno - [Register.razor](Components\Pages\Register.razor).
2. Server vygeneruje "challenge" a další potřebná data a vrátí je klientovi - [AuthController.cs](Controllers\AuthController.cs).
3. Klient použije **WebAuthn API**, které požádá uživatele o vytvoření nového passkey na jeho zařízení - [WebAuthnService.cs](Services\WebAuthnService.cs) + [webauthn.js](wwwroot\js\webauthn.js).
4. Passkey (ve formě veřejného klíče) je uložen na serveru spolu s identifikátorem uživatele - [AuthController.cs](Controllers\AuthController.cs).

### **4.2 Přihlášení**

Proces přihlášení ověřuje identitu uživatele:

1. Uživatel zadá své uživatelské jméno - [Login.razor](Components\Pages\Login.razor).
2. Server vygeneruje "challenge" a odešle ji klientovi - [AuthController.cs](Controllers\AuthController.cs).
3. Klient použije **WebAuthn API** pro ověření passkey - [WebAuthnService.cs](Services\WebAuthnService.cs) + [webauthn.js](wwwroot\js\webauthn.js).
4. Server validuje odpověď klienta a autentizuje uživatele - [AuthController.cs](Controllers\AuthController.cs).

### **4.3 Odhlášení**

Uživatel se odhlásí stisknutím tlačítka `Logout`, které zruší aktuální autentizační relaci.

---

## **5. Architektura aplikace**

### **5.1 Komponenty**

1. **Frontend:**
   - UI postavené na Blazor Serveru.
   - Minimální JavaScript pro volání WebAuthn API.

2. **Backend:**
   - API pro správu uživatelských účtů a autentifikace.
   - SQLite pro ukládání uživatelských dat.

3. **Nginx:**
   - Reverzní proxy server pro směrování požadavků na backend.
   - HTTPS terminace.

---

## **6. Uživatelské rozhraní**

### **6.1 Struktura stránek**

1. **Navigační panel:**
   Informace o aplikaci a tlačítka pro registraci/přihlášení/odhlášení.

2. **Registrace:**
   Formulář pro zadání uživatelského jména a vytvoření passkey.

3. **Přihlášení:**
   Formulář pro zadání uživatelského jména a ověření passkey.

Plus domovská stránka a 2 jednoduché stránky, které se zobrazí jen, pokud je uživatel přihlášený.

---

## **7. Instalace a nasazení**

### **7.1 Požadavky**

- .NET 9 SDK
- Nginx (pro produkční nasazení)

### **7.2 Instalace**

Viz [README.md](README.md#running-the-application).

---

## **8. Relevantní zdrojový kód**

- Front-end přihlášení a registrace: [Login.razor](Components\Pages\Login.razor) a [Register.razor](Components\Pages\Register.razor).
- Volání WebAuthn API: [WebAuthnService.cs](Services\WebAuthnService.cs) a [webauthn.js](wwwroot\js\webauthn.js). (převzato z [Fido2NetLib](https://github.com/passwordless-lib/fido2-net-lib))
- Volání backend API: [UserService.cs](Services\AuthnService.cs).
- Backend autentifikace: [AuthController.cs](Controllers\AuthController.cs).
- Konfigurace projektu: [appsettings.json](appsettings.json) a [Program.cs](Program.cs).
