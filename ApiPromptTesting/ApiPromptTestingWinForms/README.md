# API Prompt Testing Tool

A lightweight Windows Forms application for rapid iteration and testing of prompts with Perplexity APIs Sonar model. **Save time and simplify your prompt engineering workflow** by isolating prompt testing from your main application.

---

## Purpose

This tool is designed for **developers**, **researchers**, and **prompt engineers** who want a **fast, flexible way to test prompts**. Enter a system prompt and a user prompt, send them to the API, and immediately view or save the API’s response—**without touching your main application code**.

---

## Features

- **Dual prompt entry:** Separate, editable fields for system and user prompts.
- **Async API calls:** Sends prompts to the API and displays the full response.
- **Response inspection:** View raw API output in a read-only text box for easy debugging.
- **Save responses:** Optionally save API responses to a local file for analysis or sharing.
- **Minimal dependencies:** Easy to configure—just set your API endpoint and credentials in the code.
- **Simple UI:** Designed for quick, focused prompt testing.

---

## Usage

1. **Clone** or download this repository.
2. **Update** the API endpoint URL and API key in User Secrets or appsettings.json.
3. **Paste** your system and user prompts into the respective text areas.
4. **Click** **Send to API** to execute the prompt and receive the response.
5. **View** the response directly in the app or use the **Save** button to store it for later analysis.

---

## Benefits

- **Streamline prompt development:** Test and refine prompts in seconds—no full integration needed.
- **Isolate prompt logic:** Prevents prompt changes from affecting your main application.

---

## Prerequisites

- **.NET 10.0** installed.
- **Valid API credentials** for the AI chat service you want to test.

---
