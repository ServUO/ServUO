# Config file formatting

Lines starting with `#` will be read as the option description.
Blank lines will terminate the current option or description.

The syntax for setting an option is `Key=Value`.
An empty key will result in a null value where the option type is nullable, otherwise, it will use the default value.

To force an option to use its default setting, prefix it with an `@`, ex:

```
@Key=Value
```

This will ignore the current value setting and suppress config warnings about options using their default values.

