[![Build status](https://ci.appveyor.com/api/projects/status/riipy6srh0b2im01/branch/master?svg=true)](https://ci.appveyor.com/project/realmar/pipes/branch/master)
[![Build Status](https://travis-ci.org/realmar/Pipes.svg?branch=master)](https://travis-ci.org/realmar/Pipes)
[![codecov](https://codecov.io/gh/realmar/Pipes/branch/master/graph/badge.svg)](https://codecov.io/gh/realmar/Pipes)

# Pipes

`Pipes` is a library to compose modular and chainable pipes using small reusable
units called processors. Processors are statically typed, so no casting `object`s
to the their actual type!

## Getting Started

### Prerequisites

- [.NET Core](https://dotnet.github.io/)

### Installing

todo ... upload to nuget.

## Usage

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

Now let's use the just created processor:

```c#
var pipe = new Pipe<string>();

pipe.FirstConnector
    .Connect(new ParseStringProcessor<int>())
    .Finish(Console.WriteLine);

pipe.Process("2");
```

Let's see how a pipe works: (using pseudo processors)

```c#
// create a pipe which takes a list of objects of type TIn as input
var pipe new Pipe<TIn>();
    .Connect(new Processor<TIn, T1>())
    .Connect(new Processor<T1, T2>())
    .Finish(Callback<in T2>);

TIn data ...;
pipe.Process(data);
```

As can be observed, a processor takes in a given type and may output a different
type, by processing the given data.

More examples can be found in the [tests](Realmar.Pipes.Tests/Integration/PipeTests.cs).

### Variance

Processors are _variant_:

```c#
// TIn is contravariant
// TOut is covariant
IPipeProcessor<in TIn, out TOut>
```

This allows a processor to take input which is "less derived" than the outputed
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

### Junctions

Junctions are used to pass processed data from a pipe to other pipes. (or a single
pipe)

This example is taken out of the [tests](Realmar.Pipes.Tests/Integration/PipeTests.cs).
(Testcase: `Process_ConditionalJunction`) The goal is to multiply a number until it
is bigger or equal than 20, then append a string.

```c#
var mathPipe = new Pipe<double>();
var stringPipe = new Pipe<double>();

//                                             false     true        predicate
var junction = new ConditionalJunction<double>(mathPipe, stringPipe, x => x < 20);

mathPipe.FirstConnector
    .Connect(new MultiplicationProcessor(2))        // multiply with 2
    .Finish(junction.Process);

stringPipe.FirstConnector
    .Connect(new ToStringProcessor<double>())
    .Connect(new AppendStringProcessor("is your number!"))
    .Finish(results => Console.WriteLine);

mathPipe.Process(new List<double> { 1, 2, 3, 4, 5, 6 });
```

### Process Strategies

Process strategies define how the data is processed:

```c#
// SerialProcessStrategy is the default when using the parameterless constructor
var pipe = new Pipe<object>(new SerialProcessStrategy());

// Process data in parallel.
// Each element of the array passed to the pipe will be
// processed in a separate thread. All threads are synchronized at the end,
// and further processing is done in the thread where the processing was started.
var pipe = new Pipe<object>(new ParallelProcessStrategy());
```

## Running the tests

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
- Distributed junctions which allow to send the processed data to another computer for further processing
- Async/await process strategy

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.