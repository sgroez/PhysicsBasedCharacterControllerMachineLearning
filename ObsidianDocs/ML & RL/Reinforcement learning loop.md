
```mermaid
flowchart LR
      A[Agent]
      B[Environment]
      B-- state / observation -->A
      A-- action -->B
      B-- reward --->A
```