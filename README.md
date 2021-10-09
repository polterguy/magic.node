
# Magic Node

Magic Node is a simple name/value/children graph object, in addition to a _"Hyperlambda"_ parser, allowing you to
create a textual string representations of graph objects easily transformed to its relational graph object syntax,
and vice versa. This allows you to easily declaratively create execution trees using a format similar to YAML, for 
then to access every individual node, its value, name and children, from your C#, CLR code, or Hyperlambda.
For the record, Hyperlambda is _much_ easier to understand than YAML.

Hyperlambda is perfect for creating a highly humanly readable relational configuration format, or smaller
DSL engines, especially when combined with Magic Signals and Magic Lambda. Below is a small
example of Hyperlambda to give you an idea of how it looks like.

```
foo:bar
foo:int:5
foo
   child1:its-value
   child2:its-value
```

3 spaces (SP) opens up the children collection of a node, and allows you to create children associated with
some other node. In the example above, the **[child1]** and the **[child2]** nodes, have **[foo]** as their
parent. A colon `:` separates the name and the value of the node - The name is to the left of the colon, and
the value to the right.

You can optionally supply a type between a node's name and its value, which you can see above where we add
the `:int:` parts between one of our **[foo]** nodes' name and value.

To traverse the nodes later in for instance C#, you could do something such as the following.

```csharp
var root = var result = new Parser(hyperlambda).Lambda();

foreach (var idxChild in root.Children)
{
   var name = idxChild.Name;
   var value = idxChild.Value;
   /* ... etc ... */
}
```

## Supported types

Although the node structure itself can hold any value type you need inside of its `Value` property,
Hyperlambda only supports serialising the following types by default.

* `string` = System.String
* `short` = System.Int16
* `ushort` = System.UInt16
* `int` = System.Int32
* `uint` = System.UInt32
* `long` = System.Int64
* `ulong` = System.UInt64
* `decimal` = System.Decimal
* `double` = System.Double
* `single` = System.Float
* `float` = System.Float - Alias for above
* `bool` = System.Boolean
* `date` = System.DateTime - _Always_ interpreted and serialized as UTC time!
* `time` = System.TimeSpan
* `guid` = System.Guid
* `char` = System.Char
* `byte` = System.Byte
* `x` = magic.node.extensions.Expression
* `node` = magic.node.Node

The type declaration should be declared in your Hyperlambda in between the name and its value, separated by colon (:).
The default type if ommitted is `string`. An example of declaring a couple of types associated with a node's value
can be found below.

```
.foo1:int:5
.foo2:bool:true
.foo3:string:foo
.foo4:bar
```

### Extending the type system

The type system is extendible, and you can easily create support for serializing your own types, by using
the `Converter.AddConverter` method, that can be found in the `magic.node.extensions` namespace.
Below is an example of how to extend the typing system, to allow for serializing and de-serializing instances
of a `Foo` class into Hyperlambda.

```csharp
/*
 * Class you want to serialize into Hyperlambda.
 */
class Foo
{
    public int Value1 { get; set; }
    public decimal Value2 { get; set; }
}

/*
 * Adding our converter functions, and associating them
 * with a type, and a Hyperlambda type name.
 */
Converter.AddConverter(
    typeof(Foo),
    "foo",
    (obj) => {
        var foo = obj as Foo;
        return ("foo", $"{foo.Value1}-{foo.Value2}");
    }, (obj) => {
        var str = (obj as string).Split('-');
        return new Foo
        {
            Value1 = int.Parse(str[0]),
            Value2 = decimal.Parse(str[1]),
        };
    });
```

The above will allow you to serialize instances of `Foo` into your Hyperlambda, and
de-serialize these instances once needed. An example of adding a `Foo` instance into
your Hyperlambda can be found below.

```
.foo:foo:5-7
```

Later you can retrieve your `Foo` instances in your slots, using something
resembling the following, and all parsing and conversion will be automatically
taken care of.

```csharp
var foo = node.Get<Foo>();
```

## String literals

Hyperlambda also support strings the same way C# supports string, using any of the following string representations.

```
// Single quotes
foo:'howdy world this is a string'

// Double quotes
foo:"Howdy world, another string"

// Multiline strings
foo:@"Notice how the new line doesn't end the string
    here!"
```

Escape characters are supported for both single quote strings, and double quote strings, the same way they
are supported in C#, allowing you to use e.g. `\r\n` etc.

## Lambda expressions

Lambda expressions are kind of like XPath expressions, except (of course), they will references nodes
in your Node graph object, instead of XML nodes. Below is an example to give you an idea.

```
.foo:hello world
get-value:x:@.foo

// After invocation of the above slot, its value will be "hello world".
```

Most slots in Magic can accept expressions to reference nodes, values of nodes, and children of
nodes somehow. This allows you to modify the lambda graph object, as it is currently being executed,
and hence allows you to modify _"anything"_ from _"anywhere"_.

### Iterators

An expression is constructed from one or more _"iterator"_. This makes an expression
become _"dynamically chained Linq statements"_, where each iterator reacts upon
the results of its previous iterator. Each iterator takes as input an `IEnumerable`,
and returns as its result another `IEnumerable`, where the content of the iterator
somehow changes its given input, according to whatever the particular iterator's
implementation does. This approach just so happens to be perfect for retrieving
sub-sections of graph objects.

Each iterator ends with a _"/"_ or a CR/LF
sequence, and before its end, its value defines what it does. For instance the above iterator in
the __[get-value]__ invocation, starts out with a _"@"_. This implies that the iterator will find the
first node having a name of whatever follows its _"@"_. For the above this implies looking for the first
node who's name is _".foo"_. To see a slightly more advanced example, imagine the following.

```
.data
   item1:john
   item2:thomas
   item3:peter

get-value:x:@.data/*/item2
```

It might help to transform the above expression into humanly readable language. Its
English equivalent hence becomes as follows.

> Find the node with the name of '.data', then retrieve its children, and filter away everything not having a name of 'item2'

Of course, the result of the above becomes _"thomas"_.

Below is a list of all iterators that exists in magic. Substitute _"xxx"_ with any string,
and _"n"_ with any number.

* `*` Retrieves all children of its previous result.
* `#` Retrieves the value of its previous result as a node by reference.
* `-` Retrieves its previous result set's _"younger sibling"_ (previous node).
* `+` Retrieves its previous result set's _"elder sibling"_ (next node).
* `.` Retrieves its previous reult set's parent node(s).
* `..` Retrieves the root node.
* `**` Retrieves its previous result set's descendant, with a _"breadth first"_ algorithm.
* `{x}` Extrapolated expression that will be evaluated assuming it yields one result, replacing itself with the value of whatever node it points to.
* `=xxx` Retrieves the node with the _"xxx"_ value, converting to string if necessary.
* `[n,n]` Retrieves a subset of its previous result set, implying _"from, to"_ meaning \[n1,n2\>.
* `@xxx` Returns the first node _"before"_ in its hierarchy that matches the given _"xxx"_ in its name.
* `n` (any number) Returns the n'th child of its previous result set.

Notice, you can escape iterators by using backslash "\\". This allows you to look for nodes who's names
are for instance _"3"_, without using the n'th child iterator, which would defeat the purpose. In addition,
you can quote iterators by using double quotes `"`, to allow for having iterators with values that are normally
not legal within an iterator, such as `/`, etc.

Below is an example of a slightly more advanced expression.

```
.foo
   howdy:wo/rld
   jo:nothing
   howdy:earth
 
.dyn:.foo
for-each:x:@"./*/{@.dyn}/*/""=wo/rld"""
   set-value:x:@.dp/#
      :thomas was here
```

After evaluating the above Hyperlambda, the value of all nodes having _"wo/rld"_ as their value
inside of **[.foo]** will be updated to become _"thomas was here"_. Obviously, the above expression
is a ridiculous complex example, that you will probably never encounter in your own code. However,
for reference purposes, let's break it down into its individual parts.

1. Get parent node
2. Get all children
3. Filter away everything not having the name of the value of `{@.dyn}`, which resolves to the equivalent of `:x:@.dyn`, being an expression, who's result becomes _".foo"_.
4. Get its children
5. Find all nodes who's value is _"wo/rld"_.

98% of your expressions will have 1-3 iterators, no complex escaping, and no parameters.
And in fact, there's thousands of lines of Hyperlambda code in Magic, and 98% of these
expressions are as simple as follows.

```
.arguments
   foo1:string
get-value:x:@.arguments/*/foo1
```

Which translates into the following English.

> Give me the value of any **[foo1]** nodes, inside of the **[.arguments]** node.

### Extending lambda expressions/iterators

You can easily extend the expressions in this project, either with a _"static"_ iterator, implying
a direct match - Or with a dynamic parametrized iterator, allowing you to create iterators that
requires parameters, etc. To extend the supported iterators, use any of the following two static
methods.

* `Iterator.AddStaticIterator` - Creates a _"static"_ iterator, implying a direct match.
* `Iterator.AddDynamicIterator` - Creates a _"dynamic iterator create function"_.

Below is a C# example, that creates a dynamic iterator, that will only return nodes having a value,
that once converted into a string, has _exactly_ `n` characters, not less and not more.

```csharp
Iterator.AddDynamicIterator('%', (iteratorValue) => {
    var no = int.Parse(iteratorValue.Substring(1));
    return (identity, input) => {
        return input.Where(x => x.Get<string>()?.Length == no);
    };
});

var hl = @"foo
   howdy1:XXXXX
   howdy2:XXX
   howdy3:XXXXX
";
var lambda = new Parser(hl).Lambda();

var x = new Expression("../**/%3");
var result = x.Evaluate(lambda);
```

Notice how the iterator we created above, uses the `%3` parts of the expression, to parametrize
itself. If you exchange 3 with 5, it will only return **[howdy1]** and **[howdy3]** instead,
since it will look for values with 5 characters instead. The `Iterator` class can be found
in the `magic.node.extensions` namespace.

You can using the above syntax also override the default implementation of iterators, although
I wouldn't recommend it, since it would create confusion for others using your modified version.

**Notice** - To create an extension iterator is an exercise you will rarely if _ever_ need to do,
but is included here for reference purposes.

## Usage

You can include the following NuGet packages into your project to access Magic Node directly.

* `magic.node` - Core node parts.
* `magic.node.extensions` - Contains support for expressions, the Hyperlambda serializer and de-serializer, in addition to the typing system.

However, all of these packages are indirectly included when you use Magic.

## Documenting nodes, arguments to slots, etc

When referencing nodes in the documentation for Magic, it is common to reference them like __[this]__, where
_"this"_ would be the name of some node - Implying in __bold__ characters, wrapped by square [brackets].

## Project website

The source code for this repository can be found at [github.com/polterguy/magic.node](https://github.com/polterguy/magic.node), and you can provide feedback, provide bug reports, etc at the same place.

## Quality gates

- [![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=polterguy_magic.node&metric=alert_status)](https://sonarcloud.io/dashboard?id=polterguy_magic.node)
- [![Bugs](https://sonarcloud.io/api/project_badges/measure?project=polterguy_magic.node&metric=bugs)](https://sonarcloud.io/dashboard?id=polterguy_magic.node)
- [![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=polterguy_magic.node&metric=code_smells)](https://sonarcloud.io/dashboard?id=polterguy_magic.node)
- [![Coverage](https://sonarcloud.io/api/project_badges/measure?project=polterguy_magic.node&metric=coverage)](https://sonarcloud.io/dashboard?id=polterguy_magic.node)
- [![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=polterguy_magic.node&metric=duplicated_lines_density)](https://sonarcloud.io/dashboard?id=polterguy_magic.node)
- [![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=polterguy_magic.node&metric=ncloc)](https://sonarcloud.io/dashboard?id=polterguy_magic.node)
- [![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=polterguy_magic.node&metric=sqale_rating)](https://sonarcloud.io/dashboard?id=polterguy_magic.node)
- [![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=polterguy_magic.node&metric=reliability_rating)](https://sonarcloud.io/dashboard?id=polterguy_magic.node)
- [![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=polterguy_magic.node&metric=security_rating)](https://sonarcloud.io/dashboard?id=polterguy_magic.node)
- [![Technical Debt](https://sonarcloud.io/api/project_badges/measure?project=polterguy_magic.node&metric=sqale_index)](https://sonarcloud.io/dashboard?id=polterguy_magic.node)
- [![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=polterguy_magic.node&metric=vulnerabilities)](https://sonarcloud.io/dashboard?id=polterguy_magic.node)

## License

This project is the copyright(c) 2020-2021 of Thomas Hansen thomas@servergardens.com, and is licensed under the terms
of the LGPL version 3, as published by the Free Software Foundation. See the enclosed LICENSE file for details.
