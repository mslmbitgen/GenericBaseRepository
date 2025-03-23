# MB.Result NuGet Package

## Overview
The `MB.Result` package is a flexible and robust solution for encapsulating the results of operations in .NET applications. It provides a consistent way to handle both success and failure states, along with relevant data, error messages, and HTTP status codes. This package is perfect for enhancing error handling, improving response consistency, and streamlining API responses.

## Features
- **Generic Result Type**: Strongly typed to accommodate any data type for success scenarios.
- **Error Handling**: Supports multiple error messages, ideal for situations requiring detailed feedback.
- **HTTP Status Code Integration**: Ensures that operation results are aligned with HTTP status codes, making it suitable for API development.
- **Implicit Conversions**: Simplifies result creation from data or error parameters using implicit conversion operators.
- **Flexible Constructors**: Multiple constructors to handle different result types (success with data or failure with error messages).
  
## Getting Started

## Installation

To integrate `MB.Result` into your project, install it via the NuGet package manager:

```plaintext
Install-Package MB.Result
```
Or through the .NET CLI:
```plaintext
dotnet add package MB.Result
```

## Usage
- **For a successful operation**, instantiate a Result object with the desired data:

```csharp
var successResult = new Result<string>("Operation successful.");
```

- **Alternatively**, leverage implicit conversion from data:
```csharp
Result<string> result = "Operation successful.";
```

- **For error**, create a Result object with an HTTP status code and error messages:

```csharp
var errorResult = new Result<string>(400, new List<string> { "Error 1", "Error 2" });
```

## License
`MS.Result` is licensed under the MIT License. See the LICENSE file in the source repository for full details.
