# Virto Commerce File Experience API (xFiles) Module

The Virto Commerce xAPI now provides a comprehensive file upload architecture (xFiles) based on key principles to enhance developer experience, security, and extensibility. This architecture is designed to support different file providers, offer an intuitive developer experience, facilitate extensible post-processing, and seamlessly integrate with XAPI (GraphQL).

## Key Features

* **Isolation:** File upload operations are scoped, allowing for defined settings and isolation from mutations. The process involves uploading files to blob storage and then manipulating them via XAPI, creating the Xapi.FileUpload module.
* **Intuitive Developer Experience:** The file upload process supports different upload processes with customizable validation rules, such as file extensions, count, size limits, and antivirus scanning. Developers can access core validation settings by scope for client-side validation.
* **Extensible Post Processing:** File upload process supports post-processing actions like AI integration, leveraging a pipelines architecture for extensibility.
* **Ready to Use with Client Applications:** XAPI supports file usage with mutations and queries, enabling seamless integration with client applications.
* **Security:** Anonymous file uploads are disabled by default, ensuring a security-first approach. Clean answers are provided for security inquiries.

## Architecture

![image](https://github.com/VirtoCommerce/vc-module-file-experience-api/assets/7639413/b10f31da-cde9-425f-b097-5a3f026fea7e)

## Samples
The Quote module and the Frontend Application can be one of the examples how to use File Experience.

## Getting Started
1. **Register Upload Scope:** - Update appsettings.json with file upload scope settings.
  appsettings.json:
  ```json
  {
    "FileUpload": {
      "RootPath": "attachments",
      "Scopes": [
        {
          "Scope": "quote-attachments",
          "MaxFileSize": 123,
          "AllowedExtensions": [ ".jpg", ".pdf", ".png", ".txt" ]
        }
      ]
    }
  }
  ```

2. **Query Settings:** - Use GraphQL to query file upload options for the desired scope from client application.
```graphql
query {
  fileUploadOptions(scope: "quote-attachments"){
    scope
    maxFileSize
    allowedExtensions
  }
}
```
  
4. **Upload Files:** - Utilize the provided API endpoint to upload files as multipart/form-data and obtain a safe file ID.
```
POST https://<YOUR-DOMAIN>/api/files/quote-attachments
Content-Type: multipart/form-data
...
```

```cmd
curl.exe -k -F file=@test.txt https://<YOUR-DOMAIN>/api/files/quote-attachments?api_key=***
```
  
4. **Extend XAPI:** - Extend your XAPI queries and mutations to include file attachments with the safe file ID as needed for your application.

```graphql
mutation {
  addQuoteAttachments(
    command: {
      quoteId: "a73c6031-ab6a-4acc-9f16-466d287d7565"
      urls: [
        "/api/files/699fa784949a40c1acd891f74b4223d9"
        "/api/files/4c25e506a637407782bda5a5480f26a2"
      ]
    }
  )
}
```
   
6. **Implement Security:** - Implement security callback to control access to files based on your application's requirements with impementation of IFileAuthorizationRequirementFactory.
   
7. **Download Files:** - Use the provided API endpoint to download files using the safe file ID obtained during upload.
```
GET https://<YOUR-DOMAIN>/api/files/<safe-file-id>
```

8. **Delete Files:** - Use deleteFile mutation to remove file from storage.

```graphql
mutation {
  deleteFile(
    command: {
      id: "d6e575f9633946f19b9791eee0db5e1f"
    }
  )
}
```
    
## Documentation

* [Getting started](https://docs.virtocommerce.org/platform/developer-guide/GraphQL-Storefront-API-Reference-xAPI/File/getting-started/)
* [GraphQL](https://docs.virtocommerce.org/platform/developer-guide/GraphQL-Storefront-API-Reference-xAPI/File/overview/)
* [View on GitHub](https://github.com/VirtoCommerce/vc-module-file-experience-api)
* [Experience API Documentation](https://docs.virtocommerce.org/platform/developer-guide/GraphQL-Storefront-API-Reference-xAPI/)
* [How to use GraphiQL](https://docs.virtocommerce.org/platform/developer-guide/GraphQL-Storefront-API-Reference-xAPI/graphiql/)
* [How to use Postman](https://docs.virtocommerce.org/platform/developer-guide/GraphQL-Storefront-API-Reference-xAPI/postman/)
* [How to extend](https://docs.virtocommerce.org/platform/developer-guide/GraphQL-Storefront-API-Reference-xAPI/x-api-extensions/)
* [Virto Commerce Frontend architecture](https://docs.virtocommerce.org/storefront/developer-guide/architecture/)

## References

* [Deployment](https://docs.virtocommerce.org/platform/developer-guide/Tutorials-and-How-tos/Tutorials/deploy-module-from-source-code/)
* [Installation](https://docs.virtocommerce.org/platform/user-guide/modules-installation/)
* [Home](https://virtocommerce.com)
* [Community](https://www.virtocommerce.org)
* [Download latest release](https://github.com/VirtoCommerce/vc-module-file-experience-api/releases/latest)



## License
Copyright (c) Virto Solutions LTD.  All rights reserved.

This software is licensed under the Virto Commerce Open Software License (the "License"); you
may not use this file except in compliance with the License. You may
obtain a copy of the License at http://virtocommerce.com/opensourcelicense.

Unless required by the applicable law or agreed to in written form, the software
distributed under the License is provided on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
implied.
