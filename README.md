# Threema MsgApi .NET Core

This project is a .NET Core fork of the [Threema Message API SDK-NET](https://gateway.threema.ch/en/developer/sdk-net). 

The Threema Message API is an interface that can be used from within customer-specific software to send and receive messages via [Threema Gateway](https://gateway.threema.ch/en).



### Configuration for Testing


Configure in application.json:

- Threema
  - PrivateKey
  - ThreemaId
  - Secret
- Remove entry "SQLiteConnectionString" or set entry for using public key store.

