[![Build status](https://ci.appveyor.com/api/projects/status/n007fpp6670n25q1?svg=true)](https://ci.appveyor.com/project/krajek/hyperloglog)

# Introduction

This is an educational implementation of HyperLogLog algorithm based directly on:

>Flajolet et al., "HyperLogLog: the analysis of a near-optimal cardinality estimation algorithm", DMTCS proc. AH 2007, http://algo.inria.fr/flajolet/Publications/FlFuGaMe07.pdf

# Desing

I decided to split the implementation into three layers of abstraction, each with separate concern:

 * `HyperLogLogInternals` is static helper class responsible to most of the low level stateless sub-steps of the algorithm
 * `HyperLogLogCore` is the essential class that implements HyperLogLog algorithm directly on hashes, just like the HLL paper specifies
 * `HyperLogLog` is more user friendly version of HyperLogLog object, it supports serialization and hashing of most of the built-in C# data types

That way one can review the code and tests for particular aspect of HyperLogLog one at a time.

# Usage

Following code creates `HyperLogLog` object, populates it with three elements and finally retrieves calculate estimate.

```C#
var hyperLogLog = new HyperLogLog(b);
hyperLogLog.AddUTF8String("A");
hyperLogLog.AddUTF8String("B");
hyperLogLog.AddUTF8String("C");
var estimatedCount = hyperLogLog.CalculateEstimatedCount();
```

Following code creates `HyperLogLog` object, the serializes it to `byte[]`, finally deserializes back to the `HyperLogLog`.

```C#
var hyperLogLog = new HyperLogLog(b);

BinaryFormatter formatter = new BinaryFormatter();
MemoryStream stream = new MemoryStream();

formatter.Serialize(stream, hyperLogLog);
stream.Position = 0;
HyperLogLog deserialized = (HyperLogLog)formatter.Deserialize(stream);
```

# Production use

For production use it may be wiser to use https://github.com/Microsoft/CardinalityEstimation
They implement optimized version of HyperLogLog++ algorithm. It is more complex, thus harder to understand and analyze.
