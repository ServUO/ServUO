# ServUO Coding Standards

## General Conventions

### Use tabs for indents. 

### Omit `this` for member access. 

Do not use `this` to denote access to an instance's members. If `this` must be used to to access a
member, rename or refactor the code to not require it. 

**Exception**: Scripts may use `this` to denote member access.

### Use immutable data when possible. 

When possible, use `readonly` and private setters on properties to enforce immutability. 

### Do not use magic numbers or values. 

Name values appropriately to what they mean. Do not use names for very obvious meanings. 

For example, 

    private readonly int m_MageryCircles = 8;

    if (m_MageryCircles < castingSpell) 
    { 
        // something went wrong!
    }
    
    // bad examples
    if (8 < castingSpell)
    { 
        // something went wrong!
    }

    private readonly int m_one = 1; // unneeded clarification

### Do not add `using` statements that aren't used by the source file. 

Do not add unused `using` statements to the header of source files. 

## Access Control Conventions

### Fields must not be exposed as `public`, `protected`, or `internal`.

For example, 

    private int m_Gold; 

    // bad examples
    protected int m_Gold; 
    public int m_Gold;

## Naming Conventions

### Prefer descriptive names.

Use descriptive names in the ServUO code base. Use your best judgement for length. Be kind to 
yourself three months from now.

For example, 

    int goldGained;
    bool isLlamaSafe;

    // bad examples
    int gg;
    int goldGainedTodayPerCharacter;
    bool lsafe;

### Private members must be prefixed with `m_` in CamelCase. 

This rule applies to both instance and static members, including events, fields, properties, and 
methods.

For example, 

    private bool m_CanReportMurder;
    private static m_ParticleSupportType = ParticleSupportType.Detect;

    // bad examples
    private bool canReportMurder;
    private bool _canReportMurder;

### Public members must be in CamelCase. 

This rule applies to both instance and static members, including events, properties, and methods.

For example, 

    public static bool Enabled = true; 
    public int TotalGold { get; }

### Method parameters must be in pascalCase.

For example, 

    public int Method(int bridgesBurned)

    // bad examples
    public int Method(int BridgesBurned)
    public int Method(int bridgesburned)

### Access to primitive types must be done by C# primitive name. 

Do not use C# primitive types by using the .NET Framework Types. Our definition of C# primitive
types is those which have aliases within the C# language. The C# primitive types are: 

* sbyte
* byte
* short
* ushort
* int
* uint
* long
* ulong
* char
* float
* double
* bool
* decimal
* string

For example, 

    string.IsNullOrEmpty("foobar!");

    // bad example
    String.IsNullOrEmpty("foobar!");
    public Method(Int32 gold);

## ServUO Specific Guidelines

### Expose a setter on command properties if the value should be mutable in the property gump. 

Command properties are available for viewing and editing in-game by using the `[props` command. If a 
value should be mutable from this interface, expose and implement a setter on the property. Ignore 
warnings that the setter is un-used.