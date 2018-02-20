[![Build status](https://ci.appveyor.com/api/projects/status/riipy6srh0b2im01/branch/master?svg=true)](https://ci.appveyor.com/project/realmar/pipes/branch/master)
[![Build Status](https://travis-ci.org/realmar/Pipes.svg?branch=master)](https://travis-ci.org/realmar/Pipes)
[![codecov](https://codecov.io/gh/realmar/Pipes/branch/master/graph/badge.svg)](https://codecov.io/gh/realmar/Pipes)
[![CodeFactor](https://www.codefactor.io/repository/github/realmar/pipes/badge)](https://www.codefactor.io/repository/github/realmar/pipes)

# Pipes

`Realmar.Pipes` is a library to compose modular and chainable pipes using small reusable
units called processors. Processors are statically typed, therefore the need
to cast `object`s to their actual type disappears.

## Getting Started

### Prerequisites

Binaries currently target `netstandard2.0` ([.NET Core](https://dotnet.github.io/)
2.0 or .NET Framework 4.6.1), `net461`, `net46`, `net452`, `net45` and `net40`.
This means that you need one of the mentioned runtimes installed.

Mono is not targeted yet.

### Installing

`Realmar.Pipes` is available on [nuget.org](https://www.nuget.org/packages/Realmar.Pipes/).
It can be installed using following command from within Visual Studio:

```sh
PM> Install-Package Realmar.Pipes
```

### Remarks

This library is [CLS compilant](https://docs.microsoft.com/en-us/dotnet/standard/language-independence-and-language-independent-components)
which means that it is language independent. (eg. you may use it in C#, VB.NET,
IronPython, etc.)

[SemVer](https://semver.org/) is used for versioning. Currently `Realmar.Pipes`
has only prerelease versions. (0.y.z)

## Usage

### Overview

This section describes the architecture of `Realmar.Pipes`.

The idea is that a pipe is constructed using small reusable components
called processors. Data is given to the pipe which then processes the data
using the composed processors. This means that each processor is responsible to
transform the inputted data into another form.

```sh
           ┌─Pipe─────────────────────────────────┐
Input Data ⇒ [Processor]→[Processor]→[Processor] ⇒ Transformed Data
           └──────────────────────────────────────┘
```

A processor is the smallest composable unit. A single processor is
designed to do exactly one transformation on the input data.
This results in processors being very modular. Processors must be composed
using pipes. A pipe is the next bigger unit which combines multiple small
processors to perform a more complex transformation on the input data.

It is advised to design pipes in a modular way so that a pipe may be reused
on a later point.

Pipes can be connected to each other. Either by trivially giving the
transformed data to the next pipe or by using a more complex construct called
a pipe connector. Given multiple pipes, a pipe connector may decide to
which pipe it should give the data based on a predicate. Such a pipe
connector is called 'ConditionalPipeConnector'.

```sh
     ┌─Pipe─────────────────────┐ ┌─Connector─┐  True:  [ Pipe_True ]
Data ⇒ [Processor]→[Processor]  ⇒ Predicate  ⇒
     └──────────────────────────┘ └───────────┘  False: [ Pipe_False ]
```

### Processors

A processor is responsible for performing a single transformation on
the input data and return it.

Let's create a new processor which converts a string to a primitive type. This
is done by implementing the `IPipeProcessor<TIn, TOut>` interface:

```c#
public class ParseStringProcessor<TOut> : IPipeProcessor<string, TOut>
{
    public TOut Process(string data)
    {
        var tc = TypeDescriptor.GetConverter(typeof(TOut));
        var obj = tc.ConvertFromInvariantString(null, data);

        return (TOut)obj;
    }
}
```

### Pipes

A pipe is responsible to combine multiple processors into one
processing pipe line. Additionally, a pipe uses a processing strategy
which defines how the data is processed. (later more)

The processor defined above is used for this example:

```c#
// Instantiate a pipe which takes data of type string as input
var pipe = new Pipe<string>();

pipe.FirstConnector
    .Connect(new ParseStringProcessor<int>())
    .Finish(Console.WriteLine);     // print to console


pipe.Process(new List<string> { "1", "2" });

// it is also possible to just give one item to the pipe
pipe.Process("1");
```

Using pseudo processors it is possible to illustrate the usage of a pipe better:

```c#
// create a pipe which takes a list of objects of type TIn as input
var pipe new Pipe<TIn>();
    .Connect(new Processor<TIn, T1>())
    .Connect(new Processor<T1, T2>())
    .Finish(Action<IList<T2>>);

IList<TIn> data ...;
pipe.Process(data);
```

The delegate which is given to 'Finish' is the callback invoked after
the data has been processed by all processors in the specified pipe.

More examples can be found in the [tests](Realmar.Pipes.Tests/Integration/PipeTests.cs).

### Variance

Processors are _variant_:

```c#
// TIn is contravariant
// TOut is covariant
IPipeProcessor<in TIn, out TOut>
```

This allows a processor to take input which is "less derived" than the output
data of the previous processor:

```c#
class Base { }
class Derived : Base { }

// ---

// pipe takes in a list of objects of type `Derived`
var pipe = new Pipe<Derived>();

pipe.FirstConnector
    // because TIn of IPipeProcessor is contravariant
    // it is possible for this processors to take objects
    // of type `Base` as input:
    //
    // Derived --> Base
    .Connect(new Processor<Base, Base>())
    // Base --> object
    .Connect(new CastProcessor<object, Derived>())      // cast object to Derived
    // Derived --> Base
    .Connect(new Processor<Base, object>())
    .Finish(results => { });

pipe.Process(new Derived());
```

Note that this is _not possible with value types_ (eg. `int` --> `object`)
because value types are not polymorphic to object. (They are boxed instead)

### Process Strategies

Process strategies define how the data is processed:

```c#
// SerialProcessStrategy is the default when using the parameterless constructor
// of Pipe<TIn>. This strategy processes each data in the list after each other.
var pipe = new Pipe<object>(new SerialProcessStrategy());
```

```sh
        Serial
[data] ┌Strategy─┐
[data] ⇒ data   ⇒ [data][data][data]
[data] └─────────┘
```

```c#
// Process data in parallel.
// Each element of the list will be processed using the
// Parallel facility of .NET. All threads are synchronized
// at the end, and further processing is done in the thread
// where the processing was started.
var pipe = new Pipe<object>(new ParallelProcessStrategy());
```

```sh
         Parallel
       ┌─Strategy───┐
       |    data    |
[data] |   ╱    ╲   | [data]
[data] ⇒ ━ data  ━ ⇒ [data]
[data] |   ╲    ╱   | [data]
       |    data    |
       └────────────┘
```

```c#
// Use a threadpool to process the data.
// It is advised to use a NonBlockingPipe when using the ThreadPoolProcessStrategy
// as this processing strategy will not block when processing the data. (in contrast
// to the ParallelProcessStrategy which blocks until all data is processed)
// A NonBlockingPipe is optimized to work with such a strategy.
var pipe = new NonBlockingPipe<object>(new ThreadPoolProcessStrategy());
```

```sh
         ThreadPool
       ┌─Strategy───┐
       |    data ━━ ⇒ [data]
[data] |   ╱        |
[data] ⇒ ━ data ━━ ⇒ [data]
[data] |   ╲        |
       |    data ━━ ⇒ [data]
       └────────────┘
```

### Non-Blocking Pipe

By combining processors into a pipe one might see the resemblance to
Unix pipes '|'. In such a scenario the chained programs produce output
before all input is processed. Meaning that they take a stream of data as
input and give the transformed stream as output.

Using 'NonBlockingPipe's, 'Realmar.Pipes' is able provides this
functionality too!

It is possible to give a 'NonBlockingPipe' data continuously so that
the caller is not blocked.

```c#
var pipe = new NonBlockingPipe<string>();

pipe.FirstConnector
    .Connect(new ParseStringProcessor<int>())
    .Finish(Console.WriteLine);

// will not block
pipe.Process(new List<string>{ "1", "2" });

// will not block
pipe.Process(new List<string>{ "3", "4"});
```

The default processing strategy of the 'NonBlockingPipe' is the
'ThreadPoolProcessStrategy'.

It is important to note that the 'NonBlockingPipe' uses a worker thread
to manage the input data. This means that even when using the
'SerialProcessStrategy' the caller will not be blocked and
the data is processed in a separate thread. However, the pipe will not
be able to process additional data before it processed previously given
data. (Because it needs to wait for the 'SerialProcessStrategy' to finish
before it is able to give it more data.) The same applies to the 'ParallelProcessStrategy'.

### Pipe Connectors

Pipe Connectors are used to pass processed data from one pipe to other pipes.

This example is taken out of the [tests](Realmar.Pipes.Tests/Integration/PipeTests.cs).
(Testcase: `Process_ConditionalPipeConnector`) The goal is to multiply a number until it
is bigger or equal than 20, then append a string.

```c#
var mathPipe = new Pipe<double>();
var stringPipe = new Pipe<double>();

//                                                   false     true        predicate
var connector = new ConditionalPipeConnector<double>(mathPipe, stringPipe, x => x < 20);

mathPipe.FirstConnector
    .Connect(new MultiplicationProcessor(2))        // multiply with 2
    .Finish(connector.Process);

stringPipe.FirstConnector
    .Connect(new ToStringProcessor<double>())
    .Connect(new AppendStringProcessor("is your number!"))
    .Finish(results => Console.WriteLine);

mathPipe.Process(new List<double> { 1, 2, 3, 4, 5, 6 });
```

## Running the Tests

Change directory into test project and restore dependencies:

```sh
$ cd Realmar.Pipes.Tests
$ dotnet restore
```

Run the tests:

```sh
$ dotnet xunit
```

## Further Work / Ideas

- Add more processors which are useful!
- Distributed pipe connectors which allow to send the processed data to another computer for further processing

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.