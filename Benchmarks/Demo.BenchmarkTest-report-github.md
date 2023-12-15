## Version 2.0.2

```

BenchmarkDotNet v0.13.11, Windows 11 (10.0.22631.2861/23H2/2023Update/SunValley3)
AMD Ryzen 7 7800X3D, 1 CPU, 16 logical and 8 physical cores
.NET SDK 8.0.100
  [Host]   : .NET 7.0.14 (7.0.1423.51910), X64 RyuJIT AVX2 [AttachedDebugger]
  .NET 7.0 : .NET 7.0.14 (7.0.1423.51910), X64 RyuJIT AVX2
  .NET 8.0 : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI


```
| Method          | Job      | Runtime  | Mean      | Error     | StdDev    | Ratio |
|---------------- |--------- |--------- |----------:|----------:|----------:|------:|
| CreateTableTest | .NET 7.0 | .NET 7.0 | 10.034 μs | 0.1277 μs | 0.1195 μs |  1.00 |
| CreateTableTest | .NET 8.0 | .NET 8.0 |  8.100 μs | 0.1053 μs | 0.0933 μs |  0.81 |

## Version 3.0.0

- Changed the type of `ColumnWidth` from `class` to `readonly struct`

```

BenchmarkDotNet v0.13.11, Windows 11 (10.0.22631.2861/23H2/2023Update/SunValley3)
AMD Ryzen 7 7800X3D, 1 CPU, 16 logical and 8 physical cores
.NET SDK 8.0.100
  [Host]   : .NET 7.0.14 (7.0.1423.51910), X64 RyuJIT AVX2 [AttachedDebugger]
  .NET 7.0 : .NET 7.0.14 (7.0.1423.51910), X64 RyuJIT AVX2
  .NET 8.0 : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI


```
| Method          | Job      | Runtime  | Mean      | Error     | StdDev    | Ratio |
|---------------- |--------- |--------- |----------:|----------:|----------:|------:|
| CreateTableTest | .NET 7.0 | .NET 7.0 | 10.169 μs | 0.1427 μs | 0.1335 μs |  1.00 |
| CreateTableTest | .NET 8.0 | .NET 8.0 |  8.075 μs | 0.1041 μs | 0.0870 μs |  0.79 |
