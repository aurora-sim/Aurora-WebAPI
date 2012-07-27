/*
 * Copyright (c) Contributors, http://aurora-sim.org/
 * See CONTRIBUTORS.TXT for a full list of copyright holders.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *     * Redistributions of source code must retain the above copyright
 *       notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above copyright
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *     * Neither the name of the Aurora-Sim Project nor the
 *       names of its contributors may be used to endorse or promote products
 *       derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE DEVELOPERS ``AS IS'' AND ANY
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE CONTRIBUTORS BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;

using OpenMetaverse;

using Aurora.Framework;

namespace Aurora.DataManager.Migration.Migrators
{
    public class WebAPI_Migrator_0 : Migrator
    {
        public WebAPI_Migrator_0()
        {
            Version = new Version(0, 0, 0);
            MigrationName = "WebAPI";

            schema = new List<SchemaDefinition>();

            AddSchema("webapi_access_log", new ColumnDefinition[3]{
                new ColumnDefinition{
                    Name = "user",
                    Type = new ColumnTypeDef{
                        Type = ColumnType.UUID
                    }
                },
                new ColumnDefinition{
                    Name = "method",
                    Type = new ColumnTypeDef{
                        Type = ColumnType.Char,
                        Size = 32
                    }
                },
                new ColumnDefinition{
                    Name = "loggedat",
                    Type = new ColumnTypeDef{
                        Type = ColumnType.Double
                    }
                }
            }, new IndexDefinition[1]{
                new IndexDefinition{
                    Fields = new string[3]{ "user", "method", "loggedat" },
                    Type = IndexType.Primary
                }
            });

            AddSchema("webapi_access", new ColumnDefinition[]{
                new ColumnDefinition{
                    Name = "user",
                    Type = new ColumnTypeDef{
                        Type = ColumnType.UUID,
                        defaultValue = UUID.Zero.ToString()
                    }
                },
                new ColumnDefinition{
                    Name = "method",
                    Type = new ColumnTypeDef{
                        Type = ColumnType.Char,
                        Size = 32,
                        defaultValue = ""
                    }
                },
                new ColumnDefinition{
                    Name = "rate",
                    Type = new ColumnTypeDef{
                        Type = ColumnType.Integer,
                        Size = 11,
                        unsigned = true,
                        isNull = true,
                        defaultValue = null
                    }
                }
            }, new IndexDefinition[]{
                new IndexDefinition{
                    Fields = new string[2]{ "user", "method" },
                    Type = IndexType.Primary
                }
            });

            AddSchema("webapi_access_tokens", new ColumnDefinition[]{
                new ColumnDefinition{
                    Name = "user",
                    Type = new ColumnTypeDef{
                        Type = ColumnType.UUID
                    }
                },
                new ColumnDefinition{
                    Name = "accessToken",
                    Type = new ColumnTypeDef{
                        Type = ColumnType.UUID
                    }
                }
            }, new IndexDefinition[]{
                new IndexDefinition{
                    Fields = new string[1]{ "user" },
                    Type = IndexType.Primary
                }
            });
        }

        protected override void DoCreateDefaults(IDataConnector genericData)
        {
            EnsureAllTablesInSchemaExist(genericData);
            genericData.Insert("webapi_access", new object[3]{ // disables all access to the API by default
                UUID.Zero,
                "",
                null
            });
        }

        protected override bool DoValidate(IDataConnector genericData)
        {
            return TestThatAllTablesValidate(genericData);
        }

        protected override void DoMigrate(IDataConnector genericData)
        {
            DoCreateDefaults(genericData);
        }

        protected override void DoPrepareRestorePoint(IDataConnector genericData)
        {
            CopyAllTablesToTempVersions(genericData);
        }
    }
}