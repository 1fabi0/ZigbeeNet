﻿using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ZigBeeNet.Digi.XBee.CodeGenerator.Extensions;
using ZigBeeNet.Digi.XBee.CodeGenerator.Xml;

namespace ZigBeeNet.Digi.XBee.CodeGenerator
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="ZigBeeNet.Digi.XBee.CodeGenerator.ClassGenerator" />
    public class CommandGenerator : ClassGenerator
    {
        // TODO: rename packages to namespace. packages are not available in C#
        private const string _zigbeePackage = "ZigBeeNet";
        private const string _zigbeeSecurityPackage = "ZigBeeNet.Security";
        private const string _internalPackage = "ZigBeeNet.Hardware.Digi.XBee.Internal";
        private const string _commandPackage = "ZigBeeNet.Hardware.Digi.XBee.Internal.Protocol";
        private const string _enumPackage = "ZigBeeNet.Hardware.Digi.XBee.Internal.Protocol";

        private readonly Dictionary<int, string> _events = new Dictionary<int, string>();

        public void Go(Protocol protocol)
        {
            // Create "API" commands for AT commands
            foreach (Command atCommand in protocol.AT_Commands)
            {
                Parameter idParameter = new Parameter
                {
                    Name = "Frame ID",
                    DataType = "uint8",
                    Multiple = false,
                    Bitfield = false
                };

                Parameter stateParameter = new Parameter
                {
                    Name = "Command Status",
                    DataType = "CommandStatus",
                    Multiple = false,
                    Bitfield = false
                };

                Parameter atParameter = new Parameter
                {
                    Name = "AT Parameter",
                    DataType = "AtCommand",
                    Value = '"' + atCommand.CommandProperty + '"'
                };

                string description = "AT Command <b>" + atCommand.CommandProperty + "</b></p>" + atCommand.Description;

                if (atCommand.Getter)
                {
                    Command command = new Command
                    {
                        Id = 0x08,
                        Name = "Get " + atCommand.Name,
                        Description = description,
                        CommandParameters = new List<ParameterGroup>(),
                        ResponseParameters = new List<ParameterGroup>()
                    };

                    ParameterGroup commandGroup = new ParameterGroup
                    {
                        Parameters = new List<Parameter>()
                    };

                    commandGroup.Parameters.Add(idParameter);
                    commandGroup.Parameters.Add(atParameter);
                    command.CommandParameters.Add(commandGroup);
                    protocol.Commands.Add(command);
                }

                if (atCommand.Setter)
                {
                    Command command = new Command
                    {
                        Id = 0x08,
                        Name = "Set " + atCommand.Name,
                        Description = description,
                        CommandParameters = new List<ParameterGroup>(),
                        ResponseParameters = new List<ParameterGroup>()
                    };
                    ParameterGroup commandGroup = new ParameterGroup
                    {
                        Parameters = new List<Parameter>()
                    };
                    commandGroup.Parameters.Add(idParameter);
                    commandGroup.Parameters.Add(atParameter);

                    if (atCommand.CommandParameters != null && atCommand.CommandParameters.Count != 0)
                    {
                        commandGroup.Parameters.AddRange(atCommand.CommandParameters[0].Parameters);
                    }

                    command.CommandParameters.Add(commandGroup);
                    protocol.Commands.Add(command);
                }

                Command response = new Command
                {
                    Id = 0x88,
                    Name = atCommand.Name,
                    Description = description,
                    CommandParameters = new List<ParameterGroup>(),
                    ResponseParameters = new List<ParameterGroup>()
                };
                ParameterGroup responseGroup = new ParameterGroup
                {
                    Parameters = new List<Parameter>()
                };
                responseGroup.Parameters.Add(idParameter);
                responseGroup.Parameters.Add(atParameter);
                responseGroup.Parameters.Add(stateParameter);
                if (atCommand.ResponseParameters != null && atCommand.ResponseParameters.Count != 0)
                {
                    responseGroup.Parameters.AddRange(atCommand.ResponseParameters[0].Parameters);
                }
                response.ResponseParameters.Add(responseGroup);
                protocol.Commands.Add(response);
            }

            string packageName;
            string className;
            foreach (Command command in protocol.Commands)
            {
                packageName = _commandPackage;
                if (command.CommandParameters.Count > 0)
                {
                    className = "XBee" + command.Name.ToUpperCamelCase() + "Command";
                }
                else
                {
                    string responseType = "Event";
                    foreach (Parameter parameter in command.ResponseParameters[0].Parameters)
                    {
                        if (parameter.Name.ToUpper().Equals("FRAME ID"))
                        {
                            responseType = "Response";
                        }

                    }
                    className = "XBee" + command.Name.ToUpperCamelCase() + responseType;
                }

                CreateCommandClass(packageName, className, command, command.CommandParameters,
                        command.ResponseParameters);
            }

            foreach (Enumeration enumeration in protocol.Enumerations)
            {
                CreateEnumClass(enumeration);
            }

            CreateEventFactory("Event", protocol);
            CreateEventFactory("Response", protocol);
        }

        private void CreateEventFactory(string v, Protocol protocol)
        {
            throw new NotImplementedException();
        }

        private void CreateEnumClass(Enumeration enumeration)
        {
            throw new NotImplementedException();
        }

        private void CreateCommandClass(string packageName, string className, Command command, List<ParameterGroup> commandParameterGroup, List<ParameterGroup> responseParameterGroup)
        {
            if (className == "XBeeZigBeeTransmitStatusCommand")
            {
                Console.WriteLine();
            }

            if (className.EndsWith("Event"))
            {
                _events.Add(command.Id, className);
            }

            if (className.EndsWith("Response"))
            {
                // Nothing todo here
            }

            Console.WriteLine("Processing command class " + command.Name + "  [" + className + "()]");

            CreateCompileUnit(out CodeCompileUnit compileUnit, out CodeNamespace codeNamespace, "ZigBeeNet.Hardware.Digi.XBee.Internal.Protocol");
            CodeTypeDeclaration protocolClass = new CodeTypeDeclaration(className)
            {
                IsClass = true,
                TypeAttributes = System.Reflection.TypeAttributes.Public
            };

            StringBuilder descriptionStringBuilder = new StringBuilder();
            descriptionStringBuilder.AppendLine($"Class to implement the XBee command \" {command.Name} \".");
            if (!string.IsNullOrEmpty(command.Description))
            {
                OutputWithLineBreak(descriptionStringBuilder, "", command.Description);
            }
            descriptionStringBuilder.AppendLine("This class provides methods for processing XBee API commands.");
            AddCodeComment(protocolClass, descriptionStringBuilder);

            protocolClass.BaseTypes.Add("XBeeFrame");

            if (commandParameterGroup.Count > 0)
            {
                protocolClass.BaseTypes.Add("IXBeeCommand ");
            }

            if (className.EndsWith("Event"))
            {
                protocolClass.BaseTypes.Add("IXBeeEvent");
            }

            if (className.EndsWith("Response"))
            {
                protocolClass.BaseTypes.Add("IXBeeResponse ");
            }

            codeNamespace.Types.Add(protocolClass);

            CreateParameterGroups(codeNamespace, protocolClass, commandParameterGroup, null);

            CreateParameterGroups(codeNamespace, protocolClass, responseParameterGroup, (group, stringBuilder) =>
            {
                stringBuilder.AppendLine("Response field");
                if (bool.TrueString.Equals(group.Multiple))
                {
                    stringBuilder.AppendLine("Field accepts multiple responses.");
                }
            });

            foreach (ParameterGroup group in commandParameterGroup)
            {
                foreach (var parameter in group.Parameters)
                {
                    if (parameter.AutoSize != null)
                    {
                        continue;
                    }

                    // Constant...
                    if (!string.IsNullOrEmpty(parameter.Value))
                    {
                        continue;
                    }

                    StringBuilder stringBuilder = new StringBuilder();
                    if (!string.IsNullOrEmpty(parameter.Description))
                    {
                        OutputWithLineBreak(stringBuilder, "    ", parameter.Description);
                    }

                    CodeFieldReferenceExpression parameterReference = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), $"_{parameter.Name.ToLowerCamelCase()}");
                    if (parameter.Multiple || parameter.Bitfield)
                    {
                        //TODO: Eliminate redundant code below
                        //IList<string[]> methodStrings = new List<string[]>
                        //{
                        //    new[] { "Add", "", "", "Add" },
                        //    new[] { "Remove", "", "", "Remove" },
                        //    new[] { "Add", "IEnumerable<", ">", "AddRange" },
                        //};

                        //foreach(string[] methodString in methodStrings)
                        //{
                        //    CodeMemberMethod addMethod = CreateMethod($"{methodString[0]}{parameter.Name.ToUpperCamelCase()}", new CodeParameterDeclarationExpressionCollection
                        //    {
                        //        new CodeParameterDeclarationExpression(new CodeTypeReference(new CodeTypeParameter($"{methodString[1]}{GetTypeClass(parameter.DataType, codeNamespace).BaseType}{methodString[2]}")), $"{parameter.Name.ToLowerCamelCase()}")
                        //    });
                        //    CodeMethodInvokeExpression addInvoke = new CodeMethodInvokeExpression(parameterReference, $"{methodString[3]}", new CodeTypeReferenceExpression($"{parameter.Name.ToLowerCamelCase()}"));
                        //    addMethod.Statements.Add(addInvoke);
                        //    protocolClass.Members.Add(addMethod);
                        //    // Todo: Add Comment to method[0]
                        //}

                        CodeMemberMethod addMethod = CreateMethod($"Add{parameter.Name.ToUpperCamelCase()}", new CodeParameterDeclarationExpressionCollection
                        {
                            new CodeParameterDeclarationExpression(new CodeTypeReference(new CodeTypeParameter($"{GetTypeClass(parameter.DataType, codeNamespace).BaseType}")), $"{parameter.Name.ToLowerCamelCase()}")
                        });
                        CodeMethodInvokeExpression addInvoke = new CodeMethodInvokeExpression(parameterReference, "Add", new CodeTypeReferenceExpression($"{parameter.Name.ToLowerCamelCase()}"));
                        addMethod.Statements.Add(addInvoke);
                        protocolClass.Members.Add(addMethod);
                        //Todo: Add Comment to method[0]

                        CodeMemberMethod removeMethod = CreateMethod($"Remove{parameter.Name.ToUpperCamelCase()}", new CodeParameterDeclarationExpressionCollection
                        {
                            new CodeParameterDeclarationExpression(new CodeTypeReference(new CodeTypeParameter($"{GetTypeClass(parameter.DataType, codeNamespace).BaseType}")), $"{parameter.Name.ToLowerCamelCase()}")
                        });
                        CodeMethodInvokeExpression removeInvoke = new CodeMethodInvokeExpression(parameterReference, "Remove", new CodeTypeReferenceExpression($"{parameter.Name.ToLowerCamelCase()}"));
                        removeMethod.Statements.Add(removeInvoke);
                        protocolClass.Members.Add(removeMethod);
                        //Todo: Add Comment to method[0]

                        CodeMemberMethod setMethod = CreateMethod($"Set{parameter.Name.ToUpperCamelCase()}", new CodeParameterDeclarationExpressionCollection
                        {
                            new CodeParameterDeclarationExpression(new CodeTypeReference(new CodeTypeParameter($"IEnumerable<{GetTypeClass(parameter.DataType, codeNamespace).BaseType}>")), $"{parameter.Name.ToLowerCamelCase()}")
                        });
                        CodeMethodInvokeExpression setInvoke = new CodeMethodInvokeExpression(parameterReference, "AddRange", new CodeTypeReferenceExpression($"{parameter.Name.ToLowerCamelCase()}"));
                        setMethod.Statements.Add(setInvoke);
                        protocolClass.Members.Add(setMethod);
                        //Todo: Add Comment to method[0]
                    }
                    else
                    {
                        CodeMemberMethod setMethod = CreateMethod($"Set{parameter.Name.ToUpperCamelCase()}", new CodeParameterDeclarationExpressionCollection
                        {
                            new CodeParameterDeclarationExpression(new CodeTypeReference(new CodeTypeParameter($"{GetTypeClass(parameter.DataType, codeNamespace).BaseType}")), $"{parameter.Name.ToLowerCamelCase()}")
                        });

                        setMethod.Statements.Add(new CodeAssignStatement(parameterReference, new CodeArgumentReferenceExpression($"{parameter.Name.ToLowerCamelCase()}")));
                        protocolClass.Members.Add(setMethod);
                        //Todo: Add Comment to method[0]

                        if (parameter.Minimum != null && parameter.Maximum != null)
                        {
                            // TODO: See CommandGenerator.java line 613
                            // Create a CodeConditionStatement that tests a boolean value named boolean.
                            CodeConditionStatement conditionalStatement = new CodeConditionStatement(
                                // The condition to test.
                                new CodeSnippetExpression($"{parameter.Name.ToLowerCamelCase()} < {parameter.Minimum} || {parameter.Name.ToLowerCamelCase()} > {parameter.Maximum}"),
                                // The statements to execute if the condition evaluates to true.
                                new CodeStatement[] { new CodeCommentStatement("If condition is true, execute these statements.") },
                                // The statements to execute if the condition evalues to false.
                                new CodeStatement[] { new CodeCommentStatement("Else block. If condition is false, execute these statements.") });
                        }

                        if (parameter.Minimum != null)
                        {
                            // TODO: See CommandGenerator.java line 620
                            CodeConditionStatement conditionalStatement = new CodeConditionStatement(
                                // The condition to test.
                                new CodeSnippetExpression($"{parameter.Name.ToLowerCamelCase()} < {parameter.Minimum} || {parameter.Name.ToLowerCamelCase()} > {parameter.Maximum}"),
                                // The statements to execute if the condition evaluates to true.
                                new CodeStatement[] { new CodeCommentStatement("If condition is true, execute these statements.") },
                                // The statements to execute if the condition evalues to false.
                                new CodeStatement[] { new CodeCommentStatement("Else block. If condition is false, execute these statements.") });
                        }

                        if (parameter.Maximum != null)
                        {
                            // TODO: See CommandGenerator.java line 627
                            CodeConditionStatement conditionalStatement = new CodeConditionStatement(
                                // The condition to test.
                                new CodeSnippetExpression($"{parameter.Name.ToLowerCamelCase()} < {parameter.Minimum} || {parameter.Name.ToLowerCamelCase()} > {parameter.Maximum}"),
                                // The statements to execute if the condition evaluates to true.
                                new CodeStatement[] { new CodeCommentStatement("If condition is true, execute these statements.") },
                                // The statements to execute if the condition evalues to false.
                                new CodeStatement[] { new CodeCommentStatement("Else block. If condition is false, execute these statements.") });
                        }
                    }
                }
                //CreateParameterSetter(group.Parameters);
            }

            // TODO: Go on with line 249 CommndGenerator.java

            GenerateCode(compileUnit, className);
        }

        private void CreateParameterGroups(CodeNamespace codeNamespace, CodeTypeDeclaration protocolClass, IList<ParameterGroup> parameterGroups, Action<ParameterGroup, StringBuilder> action)
        {
            foreach (ParameterGroup group in parameterGroups)
            {
                foreach (var parameter in group.Parameters)
                {
                    if (parameter.AutoSize != null)
                    {
                        continue;
                    }

                    // Constant...
                    if (!string.IsNullOrEmpty(parameter.Value))
                    {
                        continue;
                    }

                    StringBuilder stringBuilder = new StringBuilder();

                    action?.Invoke(group, stringBuilder);

                    if (!string.IsNullOrEmpty(parameter.Description))
                    {
                        OutputWithLineBreak(stringBuilder, "    ", parameter.Description);
                    }
                    CreateParameterDefinition(codeNamespace, protocolClass, stringBuilder, parameter);
                }
            }
        }

        private void CreateParameterDefinition(CodeNamespace codeNamespace, CodeTypeDeclaration codeTypeDeclaration, StringBuilder codeComment, Parameter parameter)
        {
            CodeMemberField codeMemberField;
            CodeTypeReference parameterType = GetTypeClass(parameter.DataType, codeNamespace);
            if (parameter.Multiple || parameter.Bitfield)
            {
                AddNamespaceImport(codeNamespace, "System.Collections.Generic");
                codeMemberField = CreateCodeMemberField(parameter.Name, $"List<{parameterType.BaseType}>", MemberAttributes.Private, true);
            }
            else
            {
                codeMemberField = CreateCodeMemberField(parameter.Name, parameterType.BaseType, MemberAttributes.Private, false);

                // Todo: Test if the assignment works
                if (!string.IsNullOrEmpty(parameter.DefaultValue))
                {
                    CodeAssignStatement assignStatement = CreateCodeAssignStatement(parameter.Name, parameter.DefaultValue);
                }
            }
            AddCodeComment(codeMemberField, codeComment);

            codeTypeDeclaration.Members.Add(codeMemberField);
        }

        private CodeMemberProperty CreateProperty(string propertyName, CodeTypeReference propertyType, StringBuilder propertyComments, bool hasGet, bool hasSet)
        {
            CodeMemberProperty codeMemberProperty = new CodeMemberProperty
            {
                Attributes = MemberAttributes.Public | MemberAttributes.Final,
                Name = propertyName,
                HasGet = hasGet,
                HasSet = hasSet,
                Type = propertyType,
            };
            AddCodeComment(codeMemberProperty, propertyComments);
            return codeMemberProperty;
        }

        private CodeMemberMethod CreateMethod(string methodName, CodeParameterDeclarationExpressionCollection declarationExpressionCollection)
        {
            CodeMemberMethod codeMemberMethod = new CodeMemberMethod
            {
                Attributes = MemberAttributes.Public | MemberAttributes.Final,
                Name = methodName,
            };
            codeMemberMethod.Parameters.AddRange(declarationExpressionCollection);
            return codeMemberMethod;
        }

        private static void CreateCompileUnit(out CodeCompileUnit compileUnit, out CodeNamespace codeNamespace, string namespaceString)
        {
            compileUnit = new CodeCompileUnit();
            codeNamespace = new CodeNamespace("ZigBeeNet.Hardware.Digi.XBee.Internal.Protocol");
            compileUnit.Namespaces.Add(codeNamespace);
        }

        private static void AddCodeComment(CodeTypeMember codeTypeMember, StringBuilder stringBuilder)
        {
            codeTypeMember.Comments.Add(new CodeCommentStatement("<summary>", true));
            codeTypeMember.Comments.Add(new CodeCommentStatement(stringBuilder.ToString(), true));
            codeTypeMember.Comments.Add(new CodeCommentStatement("</summary>", true));
        }

        private static CodeMemberField CreateCodeMemberField(string memberName, string typeString, MemberAttributes memberAttributes, bool initializeMember)
        {
            CodeMemberField codeMemberField = new CodeMemberField
            {
                Name = $"_{memberName.ToLowerCamelCase()}",
                Type = new CodeTypeReference(new CodeTypeParameter(typeString))
            };

            if (initializeMember)
            {
                codeMemberField.InitExpression = new CodeObjectCreateExpression(typeString, new CodeExpression[] { });
            }
            codeMemberField.Attributes = MemberAttributes.Private;

            return codeMemberField;
        }

        private static CodeAssignStatement CreateCodeAssignStatement(string parameterName, object parameterValue)
        {
            return new CodeAssignStatement(new CodeVariableReferenceExpression($"_{parameterName.ToLowerCamelCase()}"), new CodePrimitiveExpression(parameterValue));
        }

        private static void GenerateCode(CodeCompileUnit codeCompileUnit, string sourceFile)
        {
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");

            if (provider.FileExtension[0] == '.')
            {
                sourceFile = $@"..\..\..\..\src\ZigBeeNet.Hardware.Digi.XBee\Internal\Protocol\{sourceFile}{provider.FileExtension}";
            }
            else
            {
                sourceFile = $@"..\..\..\..\src\ZigBeeNet.Hardware.Digi.XBee\Internal\Protocol\{sourceFile}.{provider.FileExtension}";
            }

            var codeGeneratorOptions = new CodeGeneratorOptions
            {
                BracingStyle = "C",
            };
            IndentedTextWriter tw = new IndentedTextWriter(new StreamWriter(sourceFile, false), "    ");
            provider.GenerateCodeFromCompileUnit(codeCompileUnit, tw, codeGeneratorOptions);
            tw.Close();
        }

        protected CodeTypeReference GetTypeClass(string dataType, CodeNamespace codeNamespace)
        {
            switch (dataType)
            {
                case "uint8[]":
                    {
                        return new CodeTypeReference(typeof(int[]));
                    }
                case "Data":
                    {
                        return new CodeTypeReference(typeof(int[]));
                    }
                case "uint16[]":
                    {
                        return new CodeTypeReference(typeof(int[]));
                    }
                case "uint8":
                case "uint16":
                case "Integer":
                    {
                        return new CodeTypeReference(typeof(int));
                    }
                case "Boolean":
                    {
                        return new CodeTypeReference(typeof(bool));
                    }
                case "AtCommand":
                    {
                        return new CodeTypeReference(typeof(string));
                    }
                case "String":
                    {
                        return new CodeTypeReference(typeof(string));
                    }
                case "ZigBeeKey":
                    {
                        AddNamespaceImport(codeNamespace, ($"{_zigbeeSecurityPackage}.ZigBeeKey"));
                        return new CodeTypeReference("ZigBeeKey");
                    }
                case "IeeeAddress":
                    {
                        AddNamespaceImport(codeNamespace, ($"{_zigbeePackage}.IeeeAddress"));
                        return new CodeTypeReference("IeeeAddress");
                    }
                case "ExtendedPanId":
                    {
                        AddNamespaceImport(codeNamespace, ($"{_zigbeePackage}.ExtendedPanId"));
                        return new CodeTypeReference("ExtendedPanId");
                    }
                case "ZigBeeDeviceAddress":
                    {
                        AddNamespaceImport(codeNamespace, ($"{_zigbeePackage}.ZigBeeDeviceAddress"));
                        return new CodeTypeReference("ZigBeeDeviceAddress");
                    }
                case "ZigBeeGroupAddress":
                    {
                        AddNamespaceImport(codeNamespace, ($"{_zigbeePackage}.ZigBeeGroupAddress"));
                        return new CodeTypeReference("ZigBeeGroupAddress");
                    }
                default:
                    {
                        //AddNamespaceImport(codeNamespace, ($"{_enumPackage}.{dataType}"));
                        return new CodeTypeReference(dataType);
                    }
            }
        }

        private void AddNamespaceImport(CodeNamespace codeNamespace, string namespaceToImport)
        {
            codeNamespace.Imports.Add(new CodeNamespaceImport(namespaceToImport));
        }
    }
}