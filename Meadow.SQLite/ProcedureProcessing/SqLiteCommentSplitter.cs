using System;
using System.Collections.Generic;
using System.Linq;
using Meadow.Scaffolding.Contracts;

namespace Meadow.SQLite.ProcedureProcessing
{
    public class SqLiteCommentSplitter : ICommentSplitter
    {
        private enum States
        {
            InText,
            InFirstDash,
            InDoubleDashComment,
            InSlash,
            InSlashAstrixComment,
            InAstrix
        }


        public List<ICommentSplitter.TextPart> Split(string value)
        {
            var chars =  value.Replace('\r', '\n').ToCharArray();

            var state = States.InText;

            string buffer = "";

            var parts = new List<ICommentSplitter.TextPart>();

            void Deliver(States s, bool c)
            {
                if (!string.IsNullOrEmpty(buffer))
                {
                    parts.Add(new ICommentSplitter.TextPart { Text = buffer, IsComment = c });
                }

                state = s;

                buffer = "";
            }

            foreach (var c in chars)
            {
                if (state == States.InText)
                {
                    if (c == '-')
                    {
                        state = States.InFirstDash;
                    }
                    else if (c == '/')
                    {
                        state = States.InSlash;
                    }
                    else
                    {
                        buffer += c;
                    }
                }

                if (state == States.InFirstDash)
                {
                    if (c == '-')
                    {
                        Deliver(States.InDoubleDashComment,false);
                    }
                    else
                    {
                        buffer += '-' + c;
                        state = States.InText;
                    }
                }
                if (state == States.InSlash)
                {
                    if (c == '*')
                    {
                        Deliver(States.InSlashAstrixComment,false);
                    }
                    else
                    {
                        buffer += '/' + c;
                        state = States.InText;
                    }
                }

                if (state == States.InDoubleDashComment)
                {
                    if (c == '\n' || c == '\r')
                    {
                        Deliver(States.InText,true);
                    }
                    else
                    {
                        buffer += c;
                    }
                }

                if (state == States.InSlashAstrixComment)
                {
                    if (c == '*')
                    {
                        state = States.InAstrix;
                    }
                    else
                    {
                        buffer += c;
                    }
                }

                if (state == States.InAstrix)
                {
                    if (c == '/')
                    {
                        Deliver(States.InText,true);
                    }
                    else
                    {
                        buffer += '*' + c;
                        state = States.InSlashAstrixComment;
                    }
                }
            }

            if (!string.IsNullOrEmpty(buffer))
            {
                if (state == States.InText || state==States.InFirstDash || state == States.InSlash)
                {
                    Deliver(States.InText,false);
                }else if (state == States.InDoubleDashComment || 
                          state == States.InSlashAstrixComment || 
                          state==States.InAstrix)
                {
                    Deliver(States.InText,true);
                }
            }

            return parts;
        }
    }
}