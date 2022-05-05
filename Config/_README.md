# Formatting

Lines starting with `#` will be read as the option description.

Blank lines will terminate the current option or description.

The syntax for setting an option is `Key=Value`


An empty key will result in a null value where the option type is nullable, otherwise, it will use the default value.

To force an option to use its default setting, prefix it with an `@`, ex:

```@Key=Value```

This will ignore the current value setting and suppress config warnings about options using their default values.


# DEBUG MODE

Configuration values can be overridden by a single config file: `/Config/_DEBUG.cfg`

This file should never be uploaded to your shard's repository, you should only use it for local development.

The intent of this file is to allow you to override any existing config options without editing the main config files for each system.


The options require full qualification, so you must specify the entire option key, for example;

Overriding the shard's name option would be done with the following key/value pair:

```Server.Name=Test Centre```

Note that this feature requires your server to be running in debug mode.
