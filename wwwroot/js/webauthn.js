function base64UrlToBase64(base64Url) {
  // Replace URL-safe characters with standard Base64 characters
  let base64 = base64Url.replace(/_/g, "+").replace(/-/g, "/");
  // Add padding if necessary
  while (base64.length % 4 !== 0) {
      base64 += "=";
  }
  return base64;
}

async function createCredential(options) {
  try {
    console.log("createCredential called with options:", options);

    // Convert challenge and user ID to ArrayBuffer
    options.challenge = Uint8Array.from(atob(base64UrlToBase64(options.challenge)), c => c.charCodeAt(0));
    options.user.id = Uint8Array.from(atob(base64UrlToBase64(options.user.id)), c => c.charCodeAt(0));

    // Convert excludeCredentials IDs to ArrayBuffer
    if (options.excludeCredentials) {
      options.excludeCredentials = options.excludeCredentials.map(cred => {
        return {
          ...cred,
          id: Uint8Array.from(atob(base64UrlToBase64(cred.id)), c => c.charCodeAt(0))
        };
      });
    }

    const credential = await navigator.credentials.create({ publicKey: options });

    console.log("Credential created:", credential);

    return credential;
  } catch (error) {
    console.error("Error in createCredential:", error);
    throw error;
  }
}

async function getAssertion(options) {
  try {
    console.log("getAssertion called with options:", options);

    options.challenge = Uint8Array.from(atob(base64UrlToBase64(options.challenge)), c => c.charCodeAt(0));
    options.allowCredentials.forEach(cred => {
      cred.id = Uint8Array.from(atob(base64UrlToBase64(cred.id)), c => c.charCodeAt(0));
    });

    const assertion = await navigator.credentials.get({ publicKey: options });

    console.log("Credential asserted:", assertion);

    return assertion;
  } catch (error) {
    console.error("Error in getAssertion:", error);
    throw error;
  }
}