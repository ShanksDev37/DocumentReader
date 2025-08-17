# DocumentReader

A C# console application for analyzing text documents and generating comprehensive word frequency reports.

## Overview

DocumentReader is a text analysis tool that processes text files to provide detailed statistical insights including word frequency analysis, character counts, and document metrics. The application cleans and normalizes text content, then generates organized reports with percentage breakdowns and statistical summaries.

## Features

- **Multi-format Support**: Processes TXT, CSV, and JSON files (Work in Progress)
- **Text Cleaning**: Removes special characters and normalizes whitespace
- **Word Frequency Analysis**: Groups words by occurrence count with percentage calculations
- **Statistical Reports**: Generates comprehensive document statistics
- **Organized Output**: Creates results in dedicated folders with clear naming conventions
- **Asynchronous Processing**: Uses parallel processing for improved performance on large files
- **Comprehensive Logging**: Detailed logging system for monitoring and troubleshooting

## Requirements

- .NET Framework or .NET Core
- Windows Operating System (due to file path handling)

## Output Format

The generated report includes:

### Document Statistics
- Total lines in the original file
- Total word count after processing
- Total unique word count
- Total character count (excluding line endings/ new line characters)

### Word Frequency Analysis
For each frequency group:
- Frequency of grouped words
- Percentage this group contributes to the total word count
- Count of unique words in that frequency
- Percentage this group contributes to the unique word count
- List of words, that appear in that frequency group

### Example Output Structure
```
Total Lines: (150)
Total Words: (1234)
Total Unique Words: (567)
Total Character Count: (6789)

Total entries: (5) Total Percentage: (2.025%) 
Unique Word Count: (10) Unique Word Percentage (1.764%)
(word1) (word2) (word3)...
```

## Error Handling

The application includes comprehensive error handling for:
- Invalid file paths or missing files
- Unsupported file formats
- Empty or corrupted files
- Processing errors during text analysis

All errors are logged with detailed messages and timestamps for easy troubleshooting.

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the CC-BY License.

## Version History

- **0.3**: Current version resolve small issues found when processing weird data.
- **0.2**: Previous version with enhanced async processing and organized output structure
- **0.1**: Initial release with basic text processing functionality
