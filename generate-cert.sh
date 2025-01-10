#!/bin/bash

CERT_DIR=/https
CERT_FILE=$CERT_DIR/localhost.pfx
PASSWORD=yourpassword

# Generate private key and self-signed certificate
openssl req -x509 -newkey rsa:2048 -keyout $CERT_DIR/localhost.key -out $CERT_DIR/localhost.crt -days 365 -nodes -subj "/CN=localhost"

# Create PFX file
openssl pkcs12 -export -out $CERT_FILE -inkey $CERT_DIR/localhost.key -in $CERT_DIR/localhost.crt -password pass:$PASSWORD

echo "Self-signed certificate generated at $CERT_FILE"
