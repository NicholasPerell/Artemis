// (c) Francois GUIBERT, Frozax Games
//
// Free to use for personal and commercial uses.
// Tweet @Frozax if you like it.
//

// originally with the credits above, it was used by bb to transition it from just reading out text in a single array, to a specialized system for storing text in collected classes


using System;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace Perell.Artemis
{
    public class fgCSVReader
    {

        const int FIRST_ROW_TO_READ = 2;

        public delegate void ReadLineDelegate(Line currentLine);

        public static void LoadFromFile(string file_name, int columnsToRead, ReadLineDelegate line_reader)
        {
            LoadFromString(File.ReadAllText(file_name), columnsToRead, line_reader);
        }

        public static void LoadFromString(string file_contents, int columnsToRead, ReadLineDelegate line_reader)
        {
            int file_length = file_contents.Length;

            // read char by char and when a , or \n, perform appropriate action
            int cur_file_index = 0; // index in the file
            int cur_row_index = 1;
            int cur_column_index = 1;
            Cell cur_cell = null;
            Line cur_line = new Line(columnsToRead);
            int cur_cell_index = 0;
            StringBuilder cur_item = new StringBuilder("");
            bool inside_quotes = false; // managing quotes
            while (cur_file_index < file_length)
            {
                char c = file_contents[cur_file_index++];

                switch (c)
                {
                    case '"':
                        if (!inside_quotes)
                        {
                            inside_quotes = true;
                        }
                        else
                        {
                            if (cur_file_index == file_length)
                            {
                                // end of file
                                inside_quotes = false;
                                goto case '\n';
                            }
                            else if (file_contents[cur_file_index] == '"')
                            {
                                // double quote, save one
                                cur_item.Append("\"");
                                cur_file_index++;
                            }
                            else
                            {
                                // leaving quotes section
                                inside_quotes = false;
                            }
                        }
                        break;
                    case '\r':
                        // ignore it completely
                        break;
                    case ',':
                        goto case '\n';

                    case '\n':
                        if (inside_quotes)
                        {
                            // inside quotes, this characters must be included
                            cur_item.Append(c);
                        }
                        else
                        {
                            // end of current item
                            cur_cell = new Cell(cur_item.ToString(), cur_column_index, cur_row_index);
                            cur_item.Length = 0;

                            if (cur_row_index >= FIRST_ROW_TO_READ &&
                               cur_column_index < columnsToRead + 1)
                            {
                                cur_line.SetCell(cur_column_index, cur_cell);
                            }

                            if (c == '\n' || cur_file_index == file_length)
                            {
                                // also end of line, call line reader
                                if (cur_row_index >= FIRST_ROW_TO_READ &&
                                    cur_column_index < columnsToRead + 1)
                                {
                                    line_reader(cur_line);
                                }
                                cur_column_index = 1;
                                ++cur_cell_index;
                                ++cur_row_index;
                                cur_line.Reset();
                            }
                            else
                            {
                                ++cur_column_index;
                            }
                        }
                        break;
                    default:
                        // other cases, add char
                        cur_item.Append(c);
                        break;
                }
            }
            // the while loop is done
        }
    }
}