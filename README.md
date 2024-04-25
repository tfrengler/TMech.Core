# TMech.Core

What is this? Well, it's a collection of test automation utilities centered around Selenium. It's a direct port of utilities and services that I have written for my work as a test automation engineer across various projects and collected in one place so it's easier to maintain and migrate them. They are provided here in public **as-is**, meaning I sadly do not accept suggestions or pull requests. The reason is simply that since I use this code professionally I have to be certain that it works consistently.
## TESTS:

Almost everything is covered by functional regression tests so that I can (reasonably) assure that nothing I change or fix will break stuff. Most of these tests are quite technically involved since they require a local install of all supported browsers and their webdriver binaries, as well as a Selenium Grid server. Therefore they have hardcoded values that only work on my machine.
## CONTENTS:

It consists of these bits:
1. ChromeProvider
2. Locator
3. WebdriverContext
4. FetchContext
## 1: ChromeProvider

A tool that can auto-download **Chrome for Testing** for you. This was created so that we could automatically keep a local Chrome-browser (and its webdriver binary) up to date for testing against. It's cross-platform and works on Win, Linux and Mac (only for 64-bit).
## 2: Locator

A simple static class with static methods for constructing frequently used locators such as *element id ends with x* or *element text contains x* etc.
## 3: WebdriverContext

Acts as a wrapper around a Selenium-instance. Meant for quickly and easily bootstrapping Selenium without having to deal with setting up a driverservice or browser options every time.
The whole idea was to get rid of tedious boilerplate code since initializing a Selenium/a browser is 90% of the time the same:

```C#
// Create a webdriver that runs against a locally installed browser
var WebdriverContext = WebdriverContext.CreateLocator(Browser.CHROME);
// or against a remote server ie. Selenium Grid
var WebdriverContext = WebdriverContext.CreateRemote(Browser.CHROME, new Uri("http://127.0.0.1:4444"));

// Start the webdriver binary and browser
IWebDriver Webdriver = WebdriverContext.Initialize(true);

// Do your testing here

// Clean-up
LocalWebdriverContext.Dispose();
```
## 4: FetchContext

The biggest thing in the whole package is this beast. It's not a single class but rather a library on its own encompassing multiple things. At its core it represents a context (a browser or another element) in which elements can be fetched within a timeout, and also provides conditional locating strategies for fetching elements once they fulfill certain criteria. It was built around the central premise that elements may not always be available the moment you search for them. The same mentality extends to elements. Once you've got a handle to an element a failed interaction cannot assumed to be an error because the element may have been removed and re-rendered.

Modern webpages are super complicated, and many things happen asynchronously. The days of old where you could reasonably assume that once a webpage had loaded all of its elements were ready for interaction are over. Selenium - while a great library - is also old school, and I would argue quite low level. If you call **FindElement()** it will assume the element is available otherwise you get an exception. Dealing with this is doable but can be difficult. What I experienced was that I ended up writing custom wait-methods or inline waiting code throughout my automation frameworks. It became tedious, and so I endeavored to come up with a better solution.

It's not an elegant solution by any means. It is essentially a glorified straightforward retry mechanism wrapped around finding and interacting with elements. But it works, and it works really well for all the projects it has been used for. Here's a rundown of what it can do.

The **ElementFactory** and all of its associated classes and methods are well documented, but here's a brief primer on what you can do with it and how it works:

```C#

var WebdriverContext = WebdriverContext.CreateLocator(Browser.CHROME);
IWebDriver Webdriver = WebdriverContext.Initialize(true);
// The second argument to the constructor is the max amount of time the factory should attempt to fetch elements before giving up. This timeout is propagated to all elements that it fetches.
Elements = new Utils.FetchContext(Webdriver, TimeSpan.FromSeconds(30.0d));

// Getting elements is mostly done by Fetch. Fetch tries to get the element 
// that matches the locator and will keep trying until the internal timeout is reached
// at which point an exception is thrown.
Element SomeElement = Elements.Fetch(By.Id("SomeId"));

// You can also fetch elements conditionally, that is once they fullfil certain conditions
Element SomeElement = Elements
			.FetchWhen() // This creates an instance of ElementWaiter that allows for conditional checks
			.IsEnabled(); // This only returns the element once it is enabled (this goes for input-elements)

// Once you have an element you can interact with it. All of these methods come with built in retry. Examples:

// If a Selenium exception is thrown, it will keep clicking until no exceptions are thrown or the timeout is reached
SomeElement.Click();
// Here it will keep sending the keystrokes until no exceptions are thrown or the timeout is reached
SomeElement.SendKeys("some text");
// Once again it will keep trying until no exceptions are thrown or the timeout is reached
string TheText = SomeElement.GetText();

// There's also FetchAll for getting multiple elements. You can even specify a threshold for how many there must be before returning
Element[] BunchOfElements = Elements.FetchAll(By.Id("SomeId"), 4); // Returns once at least 4 elements match the locator

// You can also select elements within the context of another element through chaining calls
Element SomeElement = Elements
			.Fetch(By.Id("SomeId"))
			.Within() // This means that all subsequent calls to Fetch, FetchWhen or FetchAll only matches elements that are children or descendants of "SomeId"
			.Fetch(By.CssSelector("[name='SomeName']"));
```

All of the element interaction methods not only retry on failure, they can also reacquire elements that are *stale* (no longer attached to the page). This even works recursively so elements acquired through multiple calls to **Within()** followed by **Fetch()** can be reacquired.

There are a number of classes for dealing with special HTML-elements, such as input-text, input-radio or -checkbox, dropdowns etc:

```C#
// input[type='text']: 
var InputTextElement = Elements.Fetch(By.Id("SomeId"));
InputTextElement
	.AsFormControl()
	.WithRobustSelection() // This means the setter - after setting the value - will double-check that the value is what you set it to otherwise retry
	.SetValue("some text");

// input[type='checkbox'] or input[type='radio']:
var InputRadioElement = Elements.Fetch(By.CssSelector("[name='RadioButton1']"));
InputRadioElement
	.AsInputRadioOrCheckbox()
	.WithRobustSelection()
	.Check();

// input[type='date']: 
var DateElement = Elements
			.Fetch(By.Id("SomeId"))
			.AsInputDate()
			.WithRobustSelection();

var MyDate = new System.DateTime(2052,12,31);
DateElement.SetDateByJS(MyDate); // Browser agnostic - and even ReactJS - friendly date-setter method

// <select>:
var DropdownElement = Elements
			.Fetch(By.Id("SomeId"));
			.AsDropdown()
			.WithRobustSelection();

// Setting an option in a dropdown
DropdownElement.SelectByText("some text");
// Getting the text of the selected option
string ChosenOptionText = DropdownElement
				.GetSelectedOption()
				.GetText();
```
