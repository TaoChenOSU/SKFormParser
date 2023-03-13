# SKFormParser
An experimental app that uses AI and the MS Semantic Kernel to perform short form parsing tasks.

## Description
This app is a proof-of-concept project that aims to demonstrate how image recognition and text completion models can be used to parse short form documents, such as invoices, receipts, or forms.
The app uses [Semantic Kernel (SK)](https://github.com/microsoft/semantic-kernel) as the foundation to orangize features into Skills/function and execute related tasks.

## How to run
```
dotnet build
dotnet run receipt --settings [path to a json file that contains endpoints and keys to the models] --local-image [path to an image file on your machine]
```

## Settings file format
```
// Azure OpenAI
{
  "type": "azure_openai",
  "azure_openai": {
    "deployment_label": "...deployment lable...",
    "model": "...the model name...",
    "endpoint": "...your model API endpoint...",
    "key": "...endpoint key..."
  },
  "recognizer": {
    "endpoint": "...Azure Cognitive Service Endpoint...",
    "key": "...endpoint key..."
  }
}

// OpenAI
{
  "type": "openai",
  "openai": {
    "model": "...the model name...",
    "key": "...openai endpoint key...",
    "organization": "...organization name...",
  },
  "recognizer": {
    "endpoint": "...Azure Cognitive Service Endpoint...",
    "key": "...endpoint key..."
  }
}
```

## Sample return results
![image](docs/images/receipt-sample.png)
```
{
  "DATE": "02/27/23",
  "LOCATION": "Seattle, 98195",
  "COMPANY": "University of Washington",
  "PHONE": "N/A",
  "ITEMS": [
    {
    "NAME": "Short-term parking tkt 1 - No. 033002",
    "PRICE": "$4.00"
    }
  ],
  "TAXES": "$1.87",
  "TOTAL": "$4.00"
}
```
