``` ini

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.14393.3930 (1607/AnniversaryUpdate/Redstone1), VM=Hyper-V
Intel Xeon CPU E5-2667 v3 3.20GHz, 1 CPU, 8 logical and 8 physical cores
  [Host]                  : .NET Framework 4.8 (4.8.4240.0), X64 RyuJIT
  MediumRun-.NET 4.7      : .NET Framework 4.8 (4.8.4240.0), X64 RyuJIT
  MediumRun-.NET Core 2.1 : .NET Core 2.1.22 (CoreCLR 4.6.29220.03, CoreFX 4.6.29220.01), X64 RyuJIT
  MediumRun-.NET Core 3.1 : .NET Core 3.1.8 (CoreCLR 4.700.20.41105, CoreFX 4.700.20.41903), X64 RyuJIT
  MediumRun-.NET Core 5.0 : .NET Core 5.0.0 (CoreCLR 5.0.20.45114, CoreFX 5.0.20.45114), X64 RyuJIT

IterationCount=15  LaunchCount=2  WarmupCount=10  

```
|                                               Method |                     Job |       Runtime | TraversalCount | VectorLength |        Mean |     Error |      StdDev |      Median |
|----------------------------------------------------- |------------------------ |-------------- |--------------- |------------- |------------:|----------:|------------:|------------:|
|                  **Google_Flatbuffers_ParseAndTraverse** |      **MediumRun-.NET 4.7** |      **.NET 4.7** |              **1** |            **3** |  **1,469.6 ns** |   **8.40 ns** |    **12.31 ns** |  **1,466.1 ns** |
|           Google_Flatbuffers_ParseAndTraversePartial |      MediumRun-.NET 4.7 |      .NET 4.7 |              1 |            3 |  1,093.1 ns |   8.61 ns |    12.62 ns |  1,095.5 ns |
|        Google_Flatbuffers_ParseAndTraverse_ObjectApi |      MediumRun-.NET 4.7 |      .NET 4.7 |              1 |            3 |  2,268.6 ns |  18.88 ns |    28.25 ns |  2,268.4 ns |
| Google_Flatbuffers_ParseAndTraversePartial_ObjectApi |      MediumRun-.NET 4.7 |      .NET 4.7 |              1 |            3 |  2,252.7 ns |  12.81 ns |    18.37 ns |  2,250.1 ns |
|                                PBDN_ParseAndTraverse |      MediumRun-.NET 4.7 |      .NET 4.7 |              1 |            3 |  4,033.5 ns |  45.78 ns |    68.52 ns |  4,038.0 ns |
|                         PBDN_ParseAndTraversePartial |      MediumRun-.NET 4.7 |      .NET 4.7 |              1 |            3 |  3,705.5 ns |  27.76 ns |    39.81 ns |  3,703.5 ns |
|                  Google_Flatbuffers_ParseAndTraverse | MediumRun-.NET Core 2.1 | .NET Core 2.1 |              1 |            3 |  1,373.0 ns |   4.68 ns |     6.40 ns |  1,375.3 ns |
|           Google_Flatbuffers_ParseAndTraversePartial | MediumRun-.NET Core 2.1 | .NET Core 2.1 |              1 |            3 |  1,009.3 ns |   4.70 ns |     6.59 ns |  1,008.8 ns |
|        Google_Flatbuffers_ParseAndTraverse_ObjectApi | MediumRun-.NET Core 2.1 | .NET Core 2.1 |              1 |            3 |  2,148.7 ns |  10.74 ns |    15.40 ns |  2,142.4 ns |
| Google_Flatbuffers_ParseAndTraversePartial_ObjectApi | MediumRun-.NET Core 2.1 | .NET Core 2.1 |              1 |            3 |  2,148.5 ns |  17.41 ns |    24.97 ns |  2,151.3 ns |
|                                PBDN_ParseAndTraverse | MediumRun-.NET Core 2.1 | .NET Core 2.1 |              1 |            3 |  4,093.0 ns |  26.59 ns |    38.97 ns |  4,084.7 ns |
|                         PBDN_ParseAndTraversePartial | MediumRun-.NET Core 2.1 | .NET Core 2.1 |              1 |            3 |  4,057.3 ns |  27.68 ns |    39.70 ns |  4,045.2 ns |
|                  Google_Flatbuffers_ParseAndTraverse | MediumRun-.NET Core 3.1 | .NET Core 3.1 |              1 |            3 |  1,372.4 ns |   9.78 ns |    13.38 ns |  1,371.3 ns |
|           Google_Flatbuffers_ParseAndTraversePartial | MediumRun-.NET Core 3.1 | .NET Core 3.1 |              1 |            3 |    980.0 ns |   3.90 ns |     5.59 ns |    979.5 ns |
|        Google_Flatbuffers_ParseAndTraverse_ObjectApi | MediumRun-.NET Core 3.1 | .NET Core 3.1 |              1 |            3 |  2,168.9 ns |  15.01 ns |    21.05 ns |  2,173.6 ns |
| Google_Flatbuffers_ParseAndTraversePartial_ObjectApi | MediumRun-.NET Core 3.1 | .NET Core 3.1 |              1 |            3 |  2,186.4 ns |  12.77 ns |    18.31 ns |  2,181.1 ns |
|                                PBDN_ParseAndTraverse | MediumRun-.NET Core 3.1 | .NET Core 3.1 |              1 |            3 |  3,430.1 ns |  10.68 ns |    14.97 ns |  3,428.1 ns |
|                         PBDN_ParseAndTraversePartial | MediumRun-.NET Core 3.1 | .NET Core 3.1 |              1 |            3 |  3,396.6 ns |  28.12 ns |    41.22 ns |  3,409.2 ns |
|                  Google_Flatbuffers_ParseAndTraverse | MediumRun-.NET Core 5.0 | .NET Core 5.0 |              1 |            3 |  1,374.2 ns |  13.19 ns |    19.34 ns |  1,374.9 ns |
|           Google_Flatbuffers_ParseAndTraversePartial | MediumRun-.NET Core 5.0 | .NET Core 5.0 |              1 |            3 |    976.9 ns |   9.54 ns |    14.27 ns |    976.9 ns |
|        Google_Flatbuffers_ParseAndTraverse_ObjectApi | MediumRun-.NET Core 5.0 | .NET Core 5.0 |              1 |            3 |  2,111.8 ns |  17.80 ns |    26.64 ns |  2,115.4 ns |
| Google_Flatbuffers_ParseAndTraversePartial_ObjectApi | MediumRun-.NET Core 5.0 | .NET Core 5.0 |              1 |            3 |  2,109.8 ns |  27.98 ns |    41.01 ns |  2,095.3 ns |
|                                PBDN_ParseAndTraverse | MediumRun-.NET Core 5.0 | .NET Core 5.0 |              1 |            3 |  3,397.8 ns |  70.23 ns |   102.94 ns |  3,467.4 ns |
|                         PBDN_ParseAndTraversePartial | MediumRun-.NET Core 5.0 | .NET Core 5.0 |              1 |            3 |  3,280.9 ns |  22.25 ns |    32.61 ns |  3,277.6 ns |
|                  **Google_Flatbuffers_ParseAndTraverse** |      **MediumRun-.NET 4.7** |      **.NET 4.7** |              **1** |           **30** | **12,310.8 ns** |  **72.94 ns** |   **106.91 ns** | **12,275.6 ns** |
|           Google_Flatbuffers_ParseAndTraversePartial |      MediumRun-.NET 4.7 |      .NET 4.7 |              1 |           30 |  8,496.3 ns |  54.05 ns |    80.90 ns |  8,493.4 ns |
|        Google_Flatbuffers_ParseAndTraverse_ObjectApi |      MediumRun-.NET 4.7 |      .NET 4.7 |              1 |           30 | 19,853.3 ns | 189.11 ns |   283.04 ns | 19,785.7 ns |
| Google_Flatbuffers_ParseAndTraversePartial_ObjectApi |      MediumRun-.NET 4.7 |      .NET 4.7 |              1 |           30 | 19,667.5 ns | 121.86 ns |   174.77 ns | 19,656.6 ns |
|                                PBDN_ParseAndTraverse |      MediumRun-.NET 4.7 |      .NET 4.7 |              1 |           30 | 26,825.6 ns | 179.86 ns |   263.63 ns | 26,733.1 ns |
|                         PBDN_ParseAndTraversePartial |      MediumRun-.NET 4.7 |      .NET 4.7 |              1 |           30 | 27,988.3 ns | 841.98 ns | 1,260.24 ns | 28,020.3 ns |
|                  Google_Flatbuffers_ParseAndTraverse | MediumRun-.NET Core 2.1 | .NET Core 2.1 |              1 |           30 | 11,641.3 ns |  59.88 ns |    89.62 ns | 11,630.2 ns |
|           Google_Flatbuffers_ParseAndTraversePartial | MediumRun-.NET Core 2.1 | .NET Core 2.1 |              1 |           30 |  8,012.7 ns |  43.45 ns |    63.68 ns |  7,985.5 ns |
|        Google_Flatbuffers_ParseAndTraverse_ObjectApi | MediumRun-.NET Core 2.1 | .NET Core 2.1 |              1 |           30 | 18,954.0 ns | 144.09 ns |   211.21 ns | 18,925.1 ns |
| Google_Flatbuffers_ParseAndTraversePartial_ObjectApi | MediumRun-.NET Core 2.1 | .NET Core 2.1 |              1 |           30 | 18,814.0 ns | 122.15 ns |   179.04 ns | 18,770.2 ns |
|                                PBDN_ParseAndTraverse | MediumRun-.NET Core 2.1 | .NET Core 2.1 |              1 |           30 | 30,280.9 ns | 195.10 ns |   285.97 ns | 30,196.0 ns |
|                         PBDN_ParseAndTraversePartial | MediumRun-.NET Core 2.1 | .NET Core 2.1 |              1 |           30 | 29,975.6 ns | 123.54 ns |   184.91 ns | 29,962.2 ns |
|                  Google_Flatbuffers_ParseAndTraverse | MediumRun-.NET Core 3.1 | .NET Core 3.1 |              1 |           30 | 11,501.1 ns |  86.47 ns |   129.42 ns | 11,477.5 ns |
|           Google_Flatbuffers_ParseAndTraversePartial | MediumRun-.NET Core 3.1 | .NET Core 3.1 |              1 |           30 |  7,550.8 ns | 119.36 ns |   174.95 ns |  7,454.7 ns |
|        Google_Flatbuffers_ParseAndTraverse_ObjectApi | MediumRun-.NET Core 3.1 | .NET Core 3.1 |              1 |           30 | 19,317.4 ns |  64.17 ns |    89.95 ns | 19,324.9 ns |
| Google_Flatbuffers_ParseAndTraversePartial_ObjectApi | MediumRun-.NET Core 3.1 | .NET Core 3.1 |              1 |           30 | 19,289.9 ns |  99.55 ns |   142.77 ns | 19,237.9 ns |
|                                PBDN_ParseAndTraverse | MediumRun-.NET Core 3.1 | .NET Core 3.1 |              1 |           30 | 24,868.0 ns | 157.17 ns |   225.41 ns | 24,882.7 ns |
|                         PBDN_ParseAndTraversePartial | MediumRun-.NET Core 3.1 | .NET Core 3.1 |              1 |           30 | 24,469.6 ns | 103.56 ns |   151.80 ns | 24,447.1 ns |
|                  Google_Flatbuffers_ParseAndTraverse | MediumRun-.NET Core 5.0 | .NET Core 5.0 |              1 |           30 | 11,507.9 ns |  71.85 ns |   105.31 ns | 11,477.7 ns |
|           Google_Flatbuffers_ParseAndTraversePartial | MediumRun-.NET Core 5.0 | .NET Core 5.0 |              1 |           30 |  7,487.6 ns |  45.97 ns |    68.81 ns |  7,464.6 ns |
|        Google_Flatbuffers_ParseAndTraverse_ObjectApi | MediumRun-.NET Core 5.0 | .NET Core 5.0 |              1 |           30 | 18,481.9 ns |  88.77 ns |   127.31 ns | 18,499.2 ns |
| Google_Flatbuffers_ParseAndTraversePartial_ObjectApi | MediumRun-.NET Core 5.0 | .NET Core 5.0 |              1 |           30 | 18,713.3 ns | 107.20 ns |   157.13 ns | 18,721.1 ns |
|                                PBDN_ParseAndTraverse | MediumRun-.NET Core 5.0 | .NET Core 5.0 |              1 |           30 | 23,644.8 ns |  95.98 ns |   140.69 ns | 23,657.3 ns |
|                         PBDN_ParseAndTraversePartial | MediumRun-.NET Core 5.0 | .NET Core 5.0 |              1 |           30 | 23,785.5 ns | 189.73 ns |   283.98 ns | 23,748.7 ns |
|                  **Google_Flatbuffers_ParseAndTraverse** |      **MediumRun-.NET 4.7** |      **.NET 4.7** |              **5** |            **3** |  **7,275.3 ns** |  **33.29 ns** |    **46.66 ns** |  **7,267.4 ns** |
|           Google_Flatbuffers_ParseAndTraversePartial |      MediumRun-.NET 4.7 |      .NET 4.7 |              5 |            3 |  5,406.9 ns |  47.13 ns |    70.55 ns |  5,401.2 ns |
|        Google_Flatbuffers_ParseAndTraverse_ObjectApi |      MediumRun-.NET 4.7 |      .NET 4.7 |              5 |            3 |  2,302.7 ns |  15.99 ns |    23.93 ns |  2,297.4 ns |
| Google_Flatbuffers_ParseAndTraversePartial_ObjectApi |      MediumRun-.NET 4.7 |      .NET 4.7 |              5 |            3 |  2,249.3 ns |  15.44 ns |    22.63 ns |  2,246.0 ns |
|                                PBDN_ParseAndTraverse |      MediumRun-.NET 4.7 |      .NET 4.7 |              5 |            3 |  3,983.3 ns |  31.83 ns |    43.57 ns |  3,992.3 ns |
|                         PBDN_ParseAndTraversePartial |      MediumRun-.NET 4.7 |      .NET 4.7 |              5 |            3 |  3,998.0 ns | 119.71 ns |   175.46 ns |  4,074.1 ns |
|                  Google_Flatbuffers_ParseAndTraverse | MediumRun-.NET Core 2.1 | .NET Core 2.1 |              5 |            3 |  7,069.3 ns |  81.20 ns |   116.45 ns |  7,017.0 ns |
|           Google_Flatbuffers_ParseAndTraversePartial | MediumRun-.NET Core 2.1 | .NET Core 2.1 |              5 |            3 |  4,910.5 ns |  18.06 ns |    25.89 ns |  4,907.2 ns |
|        Google_Flatbuffers_ParseAndTraverse_ObjectApi | MediumRun-.NET Core 2.1 | .NET Core 2.1 |              5 |            3 |  2,185.8 ns |  12.89 ns |    18.49 ns |  2,182.3 ns |
| Google_Flatbuffers_ParseAndTraversePartial_ObjectApi | MediumRun-.NET Core 2.1 | .NET Core 2.1 |              5 |            3 |  2,140.8 ns |  10.76 ns |    16.11 ns |  2,138.3 ns |
|                                PBDN_ParseAndTraverse | MediumRun-.NET Core 2.1 | .NET Core 2.1 |              5 |            3 |  4,429.3 ns |  28.46 ns |    42.60 ns |  4,429.1 ns |
|                         PBDN_ParseAndTraversePartial | MediumRun-.NET Core 2.1 | .NET Core 2.1 |              5 |            3 |  4,266.9 ns |  29.73 ns |    44.50 ns |  4,270.4 ns |
|                  Google_Flatbuffers_ParseAndTraverse | MediumRun-.NET Core 3.1 | .NET Core 3.1 |              5 |            3 |  6,806.3 ns |  19.83 ns |    28.45 ns |  6,814.2 ns |
|           Google_Flatbuffers_ParseAndTraversePartial | MediumRun-.NET Core 3.1 | .NET Core 3.1 |              5 |            3 |  4,928.8 ns |  49.33 ns |    72.31 ns |  4,934.0 ns |
|        Google_Flatbuffers_ParseAndTraverse_ObjectApi | MediumRun-.NET Core 3.1 | .NET Core 3.1 |              5 |            3 |  2,209.6 ns |   9.93 ns |    13.60 ns |  2,213.1 ns |
| Google_Flatbuffers_ParseAndTraversePartial_ObjectApi | MediumRun-.NET Core 3.1 | .NET Core 3.1 |              5 |            3 |  2,187.2 ns |  27.01 ns |    39.59 ns |  2,187.6 ns |
|                                PBDN_ParseAndTraverse | MediumRun-.NET Core 3.1 | .NET Core 3.1 |              5 |            3 |  3,728.5 ns |  22.43 ns |    32.87 ns |  3,727.0 ns |
|                         PBDN_ParseAndTraversePartial | MediumRun-.NET Core 3.1 | .NET Core 3.1 |              5 |            3 |  3,555.3 ns |  21.38 ns |    31.34 ns |  3,556.2 ns |
|                  Google_Flatbuffers_ParseAndTraverse | MediumRun-.NET Core 5.0 | .NET Core 5.0 |              5 |            3 |  6,884.1 ns |  50.47 ns |    75.54 ns |  6,887.0 ns |
|           Google_Flatbuffers_ParseAndTraversePartial | MediumRun-.NET Core 5.0 | .NET Core 5.0 |              5 |            3 |  4,852.5 ns |  52.57 ns |    77.05 ns |  4,872.2 ns |
|        Google_Flatbuffers_ParseAndTraverse_ObjectApi | MediumRun-.NET Core 5.0 | .NET Core 5.0 |              5 |            3 |  2,161.4 ns |   9.72 ns |    14.56 ns |  2,164.4 ns |
| Google_Flatbuffers_ParseAndTraversePartial_ObjectApi | MediumRun-.NET Core 5.0 | .NET Core 5.0 |              5 |            3 |  2,126.8 ns |  12.33 ns |    18.07 ns |  2,121.3 ns |
|                                PBDN_ParseAndTraverse | MediumRun-.NET Core 5.0 | .NET Core 5.0 |              5 |            3 |  3,527.5 ns |  17.56 ns |    25.74 ns |  3,519.9 ns |
|                         PBDN_ParseAndTraversePartial | MediumRun-.NET Core 5.0 | .NET Core 5.0 |              5 |            3 |  3,379.6 ns |  18.49 ns |    27.10 ns |  3,376.4 ns |
|                  **Google_Flatbuffers_ParseAndTraverse** |      **MediumRun-.NET 4.7** |      **.NET 4.7** |              **5** |           **30** | **61,729.2 ns** | **394.15 ns** |   **577.73 ns** | **61,539.5 ns** |
|           Google_Flatbuffers_ParseAndTraversePartial |      MediumRun-.NET 4.7 |      .NET 4.7 |              5 |           30 | 42,796.7 ns | 491.42 ns |   704.78 ns | 42,739.9 ns |
|        Google_Flatbuffers_ParseAndTraverse_ObjectApi |      MediumRun-.NET 4.7 |      .NET 4.7 |              5 |           30 | 20,349.3 ns | 227.99 ns |   341.25 ns | 20,258.1 ns |
| Google_Flatbuffers_ParseAndTraversePartial_ObjectApi |      MediumRun-.NET 4.7 |      .NET 4.7 |              5 |           30 | 19,876.9 ns | 150.71 ns |   216.14 ns | 19,915.1 ns |
|                                PBDN_ParseAndTraverse |      MediumRun-.NET 4.7 |      .NET 4.7 |              5 |           30 | 29,443.3 ns | 270.48 ns |   404.84 ns | 29,411.3 ns |
|                         PBDN_ParseAndTraversePartial |      MediumRun-.NET 4.7 |      .NET 4.7 |              5 |           30 | 27,776.1 ns | 165.87 ns |   243.13 ns | 27,731.6 ns |
|                  Google_Flatbuffers_ParseAndTraverse | MediumRun-.NET Core 2.1 | .NET Core 2.1 |              5 |           30 | 58,234.0 ns | 466.75 ns |   698.60 ns | 58,151.3 ns |
|           Google_Flatbuffers_ParseAndTraversePartial | MediumRun-.NET Core 2.1 | .NET Core 2.1 |              5 |           30 | 40,148.2 ns | 372.17 ns |   545.52 ns | 40,315.0 ns |
|        Google_Flatbuffers_ParseAndTraverse_ObjectApi | MediumRun-.NET Core 2.1 | .NET Core 2.1 |              5 |           30 | 19,341.8 ns | 122.51 ns |   183.37 ns | 19,309.6 ns |
| Google_Flatbuffers_ParseAndTraversePartial_ObjectApi | MediumRun-.NET Core 2.1 | .NET Core 2.1 |              5 |           30 | 19,031.3 ns | 165.85 ns |   243.11 ns | 18,939.2 ns |
|                                PBDN_ParseAndTraverse | MediumRun-.NET Core 2.1 | .NET Core 2.1 |              5 |           30 | 32,866.0 ns | 237.29 ns |   355.16 ns | 32,810.6 ns |
|                         PBDN_ParseAndTraversePartial | MediumRun-.NET Core 2.1 | .NET Core 2.1 |              5 |           30 | 31,044.8 ns | 160.45 ns |   230.11 ns | 31,059.0 ns |
|                  Google_Flatbuffers_ParseAndTraverse | MediumRun-.NET Core 3.1 | .NET Core 3.1 |              5 |           30 | 57,110.0 ns | 288.98 ns |   432.53 ns | 56,989.8 ns |
|           Google_Flatbuffers_ParseAndTraversePartial | MediumRun-.NET Core 3.1 | .NET Core 3.1 |              5 |           30 | 38,316.1 ns | 195.68 ns |   292.89 ns | 38,263.0 ns |
|        Google_Flatbuffers_ParseAndTraverse_ObjectApi | MediumRun-.NET Core 3.1 | .NET Core 3.1 |              5 |           30 | 19,293.4 ns |  99.90 ns |   146.44 ns | 19,321.2 ns |
| Google_Flatbuffers_ParseAndTraversePartial_ObjectApi | MediumRun-.NET Core 3.1 | .NET Core 3.1 |              5 |           30 | 19,041.8 ns | 112.79 ns |   165.33 ns | 19,073.1 ns |
|                                PBDN_ParseAndTraverse | MediumRun-.NET Core 3.1 | .NET Core 3.1 |              5 |           30 | 27,393.3 ns | 197.34 ns |   283.03 ns | 27,285.7 ns |
|                         PBDN_ParseAndTraversePartial | MediumRun-.NET Core 3.1 | .NET Core 3.1 |              5 |           30 | 25,690.7 ns | 300.03 ns |   439.78 ns | 25,501.6 ns |
|                  Google_Flatbuffers_ParseAndTraverse | MediumRun-.NET Core 5.0 | .NET Core 5.0 |              5 |           30 | 56,626.8 ns | 250.42 ns |   367.06 ns | 56,604.8 ns |
|           Google_Flatbuffers_ParseAndTraversePartial | MediumRun-.NET Core 5.0 | .NET Core 5.0 |              5 |           30 | 37,534.1 ns | 109.74 ns |   160.86 ns | 37,550.8 ns |
|        Google_Flatbuffers_ParseAndTraverse_ObjectApi | MediumRun-.NET Core 5.0 | .NET Core 5.0 |              5 |           30 | 19,019.3 ns | 118.29 ns |   177.05 ns | 19,018.1 ns |
| Google_Flatbuffers_ParseAndTraversePartial_ObjectApi | MediumRun-.NET Core 5.0 | .NET Core 5.0 |              5 |           30 | 18,920.4 ns |  84.87 ns |   127.03 ns | 18,899.9 ns |
|                                PBDN_ParseAndTraverse | MediumRun-.NET Core 5.0 | .NET Core 5.0 |              5 |           30 | 26,117.6 ns | 103.22 ns |   148.04 ns | 26,102.3 ns |
|                         PBDN_ParseAndTraversePartial | MediumRun-.NET Core 5.0 | .NET Core 5.0 |              5 |           30 | 24,584.6 ns | 165.33 ns |   247.46 ns | 24,495.3 ns |
