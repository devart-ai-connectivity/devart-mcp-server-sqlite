// --------------------------------------------------------------------------
// <copyright file="McpSqlStatementsParser.cs" company="Devart">
//
// Copyright (c) Devart. ALL RIGHTS RESERVED
// Use of the source code is permitted under the license.
// </copyright>
// --------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Devart.AI.McpServer.Interfaces;

namespace Devart.AI.McpServer
{
  public sealed class McpSqlStatementsParser : ISqlStatementsParser
  {
    private const string
      SelectKeyword = "SELECT",
      InsertKeyword = "INSERT",
      UpdateKeyword = "UPDATE",
      DeleteKeyword = "DELETE",
      CreateKeyword = "CREATE",
      AlterKeyword = "ALTER",
      DropKeyword = "DROP",
      CallKeyword = "CALL",
      ExecKeyword = "EXEC",
      ExecuteKeyword = "EXECUTE";

    public SqlStatement[] Parse(string scriptText)
    {
      if (string.IsNullOrWhiteSpace(scriptText))
      {
        return [];
      }

      var statements = new List<SqlStatement>();
      var position = 0;

      while (position < scriptText.Length)
      {
        var statementText = GetNextStatement(scriptText, ref position);
        if (!string.IsNullOrWhiteSpace(statementText))
        {
          var statementType = GetStatementType(statementText);
          statements.Add(new SqlStatement { Text = statementText, Type = statementType });
        }
      }

      return [.. statements];
    }

    private static StatementType GetStatementType(string statementText)
    {
      if (statementText.StartsWith(SelectKeyword, StringComparison.OrdinalIgnoreCase))
      {
        return StatementType.Select;
      }
      if (statementText.StartsWith(InsertKeyword, StringComparison.OrdinalIgnoreCase))
      {
        return StatementType.Insert;
      }
      if (statementText.StartsWith(UpdateKeyword, StringComparison.OrdinalIgnoreCase))
      {
        return StatementType.Update;
      }
      if (statementText.StartsWith(DeleteKeyword, StringComparison.OrdinalIgnoreCase))
      {
        return StatementType.Delete;
      }
      if (statementText.StartsWith(CreateKeyword, StringComparison.OrdinalIgnoreCase))
      {
        return StatementType.Create;
      }
      if (statementText.StartsWith(AlterKeyword, StringComparison.OrdinalIgnoreCase))
      {
        return StatementType.Alter;
      }
      if (statementText.StartsWith(DropKeyword, StringComparison.OrdinalIgnoreCase))
      {
        return StatementType.Drop;
      }
      if (statementText.StartsWith(CallKeyword, StringComparison.OrdinalIgnoreCase) ||
          statementText.StartsWith(ExecKeyword, StringComparison.OrdinalIgnoreCase) ||
          statementText.StartsWith(ExecuteKeyword, StringComparison.OrdinalIgnoreCase))
      {
        return StatementType.Call;
      }

      return StatementType.Unknown;
    }

    private static string GetNextStatement(string scriptText, ref int position)
    {
      SkipWhitespaceAndComments(scriptText, ref position);

      if (position >= scriptText.Length)
      {
        return null;
      }

      var startPosition = position;

      while (position < scriptText.Length)
      {
        var currentChar = scriptText[position];

        if (currentChar == '\'' || currentChar == '"')
        {
          SkipStringLiteral(scriptText, ref position, currentChar);
          continue;
        }

        if (currentChar == '-' && position + 1 < scriptText.Length && scriptText[position + 1] == '-')
        {
          SkipLineComment(scriptText, ref position);
          continue;
        }

        if (currentChar == '/' && position + 1 < scriptText.Length && scriptText[position + 1] == '*')
        {
          SkipBlockComment(scriptText, ref position);
          continue;
        }

        if (currentChar == ';')
        {
          if (position > startPosition)
          {
            var statementText = scriptText[startPosition..position].Trim();
            position++;

            return statementText;
          }
          else
          {
            position++;
            return GetNextStatement(scriptText, ref position);
          }
        }

        position++;
      }

      if (position > startPosition)
      {
        var statementText = scriptText[startPosition..position].Trim();
        if (!string.IsNullOrWhiteSpace(statementText))
        {
          return statementText;
        }
      }

      return null;
    }

    private static void SkipWhitespaceAndComments(string scriptText, ref int position)
    {
      while (position < scriptText.Length)
      {
        var currentChar = scriptText[position];

        if (char.IsWhiteSpace(currentChar))
        {
          position++;
        }
        else if (currentChar == '-' && position + 1 < scriptText.Length && scriptText[position + 1] == '-')
        {
          SkipLineComment(scriptText, ref position);
        }
        else if (currentChar == '/' && position + 1 < scriptText.Length && scriptText[position + 1] == '*')
        {
          SkipBlockComment(scriptText, ref position);
        }
        else
        {
          break;
        }
      }
    }

    private static void SkipStringLiteral(string scriptText, ref int position, char quote)
    {
      position++;

      while (position < scriptText.Length)
      {
        var currentChar = scriptText[position];

        if (currentChar == quote)
        {
          if (position + 1 < scriptText.Length && scriptText[position + 1] == quote)
          {
            position += 2;
          }
          else
          {
            position++;
            break;
          }
        }
        else
        {
          position++;
        }
      }
    }

    private static void SkipLineComment(string scriptText, ref int position)
    {
      while (position < scriptText.Length && scriptText[position] != '\n')
      {
        position++;
      }
    }

    private static void SkipBlockComment(string scriptText, ref int position)
    {
      position += 2;

      while (position + 1 < scriptText.Length)
      {
        if (scriptText[position] == '*' && scriptText[position + 1] == '/')
        {
          position += 2;
          break;
        }
        position++;
      }
    }
  }
}
