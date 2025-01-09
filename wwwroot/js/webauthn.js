// this file is a copy of https://github.com/passwordless-lib/fido2-net-lib/blob/master/Src/Fido2.BlazorWebAssembly/wwwroot/js/WebAuthn.ts
export function isWebAuthnPossible() {
    return !!window.PublicKeyCredential;
}

function toBase64Url(arrayBuffer) {
  return btoa(String.fromCharCode(...new Uint8Array(arrayBuffer))).replace(/\+/g, "-").replace(/\//g, "_").replace(/=*$/g, "");
}
function fromBase64Url(value) {
  return Uint8Array.from(atob(value.replace(/-/g, "+").replace(/_/g, "/")), c => c.charCodeAt(0));
}
function base64StringToUrl(base64String) {
  return base64String.replace(/\+/g, "-").replace(/\//g, "_").replace(/=*$/g, "");
}

export async function createCredential(options) {
  try {
    console.log("createCredential called with options:", options);

    if (typeof options.challenge === 'string')
      options.challenge = fromBase64Url(options.challenge);
    if (typeof options.user.id === 'string')
      options.user.id = fromBase64Url(options.user.id);
    if (options.rp.id === null)
      options.rp.id = undefined;
    for (let cred of options.excludeCredentials) {
      if (typeof cred.id === 'string')
        cred.id = fromBase64Url(cred.id);
    }

    const credential = await navigator.credentials.create({ publicKey: options });

    console.log("Credential created:", credential);

    const response = credential.response;

    const retval = {
      id: base64StringToUrl(credential.id),
      rawId: toBase64Url(credential.rawId),
      type: credential.type,
      clientExtensionResults: credential.getClientExtensionResults(),
      response: {
        attestationObject: toBase64Url(response.attestationObject),
        clientDataJSON: toBase64Url(response.clientDataJSON),
        transports: response.getTransports ? response.getTransports() : []
      }
    };

		return retval;
  } catch (error) {
    console.error("Error in createCredential:", error);
    throw error;
  }
}

export async function getAssertion(options) {
  try {
    console.log("getAssertion called with options:", options);

    if (typeof options.challenge === 'string')
      options.challenge = fromBase64Url(options.challenge);
    if (options.allowCredentials) {
      for (var i = 0; i < options.allowCredentials.length; i++) {
        const id = options.allowCredentials[i].id;
        if (typeof id === 'string')
          options.allowCredentials[i].id = fromBase64Url(id);
      }
    }

    const credential = await navigator.credentials.get({ publicKey: options });

    console.log("Credential asserted:", credential);

    const response = credential.response;

    const retval = {
      id: credential.id,
      rawId: toBase64Url(credential.rawId),
      type: credential.type,
      clientExtensionResults: credential.getClientExtensionResults(),
      response: {
        authenticatorData: toBase64Url(response.authenticatorData),
        clientDataJSON: toBase64Url(response.clientDataJSON),
        userHandle: response.userHandle && response.userHandle.byteLength > 0 ? toBase64Url(response.userHandle) : undefined,
        signature: toBase64Url(response.signature)
      }
    }

    return retval;
  } catch (error) {
    console.error("Error in getAssertion:", error);
    throw error;
  }
}