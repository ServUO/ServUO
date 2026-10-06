EXENAME=ServUO
CONFIG=release
BUILD_DIR=${CURDIR}
EXE=$(BUILD_DIR)/$(EXENAME).exe

.PHONY: all build run clean release debug

all: build run

release: CONFIG = release
release: run

debug: CONFIG = debug
debug: run

build: 
	@echo "Compile $(EXENAME) for Linux ($(CONFIG))"
	@dotnet build -c $(CONFIG)
	@echo "Done!"

run: build
	@if [ -f "$(EXE)" ]; then \
		clear; \
		echo "Running $(EXENAME) in $(CONFIG) mode..."; \
		mono $(EXE) -$(CONFIG); \
	else \
		echo "Executable not found. Did the build succeed?"; \
		exit 1; \
	fi

clean:
	@dotnet clean
	@echo "Cleaned build files."
