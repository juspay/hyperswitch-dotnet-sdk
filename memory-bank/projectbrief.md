# Project Brief

## Core Requirements & Goals

The primary goal of this project is to develop and maintain a .NET SDK for interacting with the Hyperswitch payments platform. A key deliverable is a demonstration ASP.NET Core Minimal API application that showcases the SDK's capabilities for various payment flows. This demo application will allow for manual testing and validation of the SDK.

Current Task: Create a .NET backend (Minimal API) to manually test SDK payment flows.

## Project Scope

**Included:**
*   Development of the Hyperswitch .NET SDK (`Hyperswitch.Sdk`).
*   Development of a sample console application for automated SDK testing (`Hyperswitch.Sdk.Sample`).
*   Development of a demo ASP.NET Core Minimal API application (`Hyperswitch.Sdk.DemoApi`) for manual SDK testing.
*   Implementation of core payment flows (create, confirm, capture, void, refund, sync) within the SDK and demo API.
*   Documentation within the Memory Bank system.

**Excluded:**
*   UI for the demo API. Testing will be done via API client tools (e.g., Postman).
*   Production deployment of the demo API.
*   Advanced features beyond core payment flows unless specifically requested.

## Key Stakeholders

*   Development team (responsible for SDK and demo app).
*   Users of the Hyperswitch .NET SDK.
