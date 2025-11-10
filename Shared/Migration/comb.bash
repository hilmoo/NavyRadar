#!/bin/bash

MIGRATION_DIR="." 
FILE_PATTERN="*.sql"
OUTPUT_FILE="0_combined.sql"

OUTPUT_PATH="$MIGRATION_DIR/$OUTPUT_FILE"

if [ -f "$OUTPUT_PATH" ]; then
    rm "$OUTPUT_PATH"
fi

echo "Creating combined file at $OUTPUT_PATH..."

for f in $(ls -v $MIGRATION_DIR/$FILE_PATTERN); do
    if [ -r "$f" ]; then
        echo "Appending $f..."
        
        cat "$f" >> "$OUTPUT_PATH"
        
        printf " " >> "$OUTPUT_PATH"
    fi
done

echo "" >> "$OUTPUT_PATH"

echo "Done. All files combined into $OUTPUT_PATH"