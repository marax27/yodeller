# Terminology

- **Stubs** provide canned answers to calls made during the test, usually not responding at all to anything outside what's programmed in for the test. Stubs may also record information about calls, such as an email gateway stub that remembers the messages it 'sent', or maybe only how many messages it 'sent'. Source: https://www.martinfowler.com/articles/mocksArentStubs.html

- **Media Locator** - an ID or an URL that Yodeller will use to download a media instance. Initially, it was supposed to be named "Media ID" but:

1. Requests would have two kinds of ID: a regular `ID`, and a `Media ID`. That would be confusing.
2. The name `Locator` is inspired by the URL (Uniform Resource Locator) - a way to locate a resource.
