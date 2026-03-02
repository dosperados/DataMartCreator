#DMCApp 

```JSON
{
  "application": "DMCApp",
  "layout": {
    "leftPanel": {
      "description": "Вкладки как в Visual Studio (Solution Explorer, Properties)",
      "tabs": [
        {
          "id": "TableDefine",
          "uiElements": {
            "topFilter": {
              "description": "Фильтрация списка имен таблиц",
              "source": "[Setting].[TableDefine]",
              "filterField": "[DestMainTableName]"
            },
            "bottomButton": {
              "label": "Add new",
              "action": "StartWizard"
            }
          },
          "wizard": {
            "step1": {
              "title": "Add new TableDefine",
              "infoString": "Step 1: Source and Destination scope",
              "description": "Specify the Source and Destination Tables.",
              "navigation": { "back": true },
              "fields": [
                {
                  "name": "Source DB",
                  "source": "Settings (Source DB only) OR Manual Input",
                  "maxLength": 128
                },
                {
                  "name": "Source Schema",
                  "source": "SQL - GetSchema (based on Source DB) OR Manual Input",
                  "maxLength": 128
                },
                {
                  "name": "Source Table",
                  "source": "SQL - GetTableNames (with filter) OR Manual Input",
                  "maxLength": 128
                },
                {
                  "name": "Dest DB",
                  "source": "Manual Input",
                  "defaultValue": "TADM",
                  "maxLength": 128
                },
                {
                  "name": "Dest Schema",
                  "source": "Manual Input",
                  "defaultValue": "dbo",
                  "maxLength": 128
                },
                {
                  "name": "Dest Table",
                  "source": "Manual Input",
                  "defaultValue": "dim",
                  "maxLength": 128
                }
              ],
              "checkboxes": [
                {
                  "label": "Add 'OptionId' Column",
                  "default": true,
                  "logic": "Adds records to [ReferenceTableDefine], [ReferenceColumnDefine], [ColumnDefine] for AOL link"
                },
                {
                  "label": "Add '@CompanyId'",
                  "default": true,
                  "logic": "Adds link by @CompanyId in [TADM].[Setting].[ReferenceColumnDefine]"
                }
              ],
              "buttons": {
                "addDefaultTableReference": {
                  "label": "Add Default Table Reference",
                  "action": "Open Popup [dbo].[DefaultTableReference]",
                  "description": "Edit table to add extra mappings.",
                  "examplesIncludedInSpec": [
                    {
                      "id": 1,
                      "map": "ServiceInvoiceLine (CustomerNo) -> dimServiceInvoiceLine (SelltoCustomerNo) via Customer (No->CustomerId)"
                    },
                    {
                      "id": 2,
                      "map": "ServiceInvoiceLine (BilltoCustomerNo) -> dimServiceInvoiceLine (BilltoCustomerNo) via Customer (No->CustomerId)"
                    },
                    {
                      "id": 3,
                      "map": "ServiceInvoiceLine (ItemCategoryCode) -> dimServiceInvoiceLine (ItemCategoryCode) via ItemCategory (Code->ItemCategoryId)"
                    },
                    {
                      "id": 4,
                      "map": "ServiceInvoiceLine (ServiceItemNo) -> dimServiceInvoiceLine (ServiceItemNo) via ServiceItem (No->ServiceItemId)"
                    }
                  ]
                }
              },
              "bottomNavigation": {
                "next": { "condition": "All 6 DB/Schema/Table fields must be filled" },
                "cancel": true
              }
            },
            "step2": {
              "title": "Add new TableDefine",
              "infoString": "Step 2: Column Definitions",
              "description": "Review columns. Blacklisted items are gray/strikethrough but can be enabled via 'AddToDest'.",
              "navigation": { "back": true },
              "dataGrid": {
                "sourceQuery": "SQL - GetSourceColumn",
                "columns": [
                  { "name": "TableDefineId", "visible": false },
                  { "name": "ReferenceTableDefineId", "visible": false },
                  { "name": "SourceDBId", "visible": false },
                  { "name": "SourceDBName", "visible": false },
                  { "name": "SourceTableId", "visible": false },
                  { "name": "SourceTableName", "visible": false },
                  { "name": "SourceColumnId", "visible": false },
                  { "name": "SourceColumnName", "label": "Source Column", "editable": false },
                  { "name": "CustomeStatment", "label": "Custome Statment", "editable": true },
                  { "name": "DestDBId", "visible": false },
                  { "name": "DestDBName", "visible": false },
                  { "name": "DestTableId", "visible": false },
                  { "name": "DestTableName", "visible": false },
                  { "name": "DestColumnId", "visible": false },
                  {
                    "name": "DestColumnName",
                    "label": "Dest column",
                    "editable": true,
                    "logic": [
                      "Check [Setting].[DefaultColumnNameChange] for match.",
                      "If starts with 'TA' -> remove 'TA' (UNLESS IsOptionField=1).",
                      "Check for 'Orig' prefix logic."
                    ]
                  },
                  { "name": "ColumnSortNo", "label": "Column Sort", "editable": true },
                  {
                    "name": "IsOptionField",
                    "label": "Is Option Field",
                    "type": "checkbox",
                    "logic": "Set to 1 if SourceColumnName matches any [FieldNameDWH] from 'SQL - GetAOL'"
                  },
                  {
                    "name": "FieldType",
                    "label": "Field Type",
                    "editable": true,
                    "options": ["OptionNo", "OptionId", "OptionDE", "OptionENU", "OptionHashId", "RefId", "NULL"],
                    "default": "NULL"
                  },
                  {
                    "name": "DataType",
                    "label": "Data Type",
                    "editable": true,
                    "storage": "settings.json",
                    "autoLogic": [
                      "IF SourceColumnName like '%Date' AND DataType='datetime' THEN 'DATE'",
                      "IF DataType='decimal' AND Precision=38 THEN Precision=18, Scale=2"
                    ],
                    "validation": [
                      { "types": ["VARBINARY", "nvarchar", "char"], "set": { "MaxLength": 1, "Precision": null, "Scale": null, "Collation": "Latin1_General_CI_AS" } },
                      { "type": "tinyint", "set": { "MaxLength": null, "Precision": 3, "Scale": 0 } },
                      { "type": "int", "set": { "MaxLength": null, "Precision": 10, "Scale": 0 } },
                      { "type": "bigint", "set": { "MaxLength": null, "Precision": 19, "Scale": 0 } },
                      { "type": "decimal", "set": { "MaxLength": null, "Precision": 18, "Scale": 2 } },
                      { "type": "DATE", "set": { "MaxLength": null, "Precision": null, "Scale": null } },
                      { "type": "uniqueidentifier", "set": { "MaxLength": null, "Precision": null, "Scale": null } }
                    ]
                  },
                  { "name": "MaxLength", "label": "Max Length", "editable": true },
                  { "name": "Precision", "label": "Precision", "editable": true },
                  { "name": "Scale", "label": "Scale", "editable": true },
                  { "name": "IsNullable", "label": "Is Nullable", "type": "checkbox", "editable": true },
                  { "name": "CollationName", "label": "Collation", "editable": true, "options": ["Latin1_General_CI_AS"] },
                  { "name": "DefaultValue", "label": "Default Value", "editable": true },
                  { "name": "KeySortNo", "label": "Key Sort No", "editable": true },
                  { "name": "SourceKeyColumn", "label": "Source Key Column", "editable": true },
                  { "name": "DestKeyColumn", "label": "Dest Key Column", "editable": true },
                  { "name": "RefTableName", "label": "Ref Table", "editable": true, "default": "NULL" },
                  { "name": "RefColumnName", "label": "Ref Column", "editable": true },
                  { "name": "RefLevelNo", "label": "Ref Level No", "editable": true, "default": "NULL" },
                  {
                    "name": "AddToDest",
                    "label": "Add",
                    "type": "checkbox",
                    "editable": true,
                    "logic": {
                      "init": "1 unless in [dbo].[DefaultColumnBlackList]",
                      "visual": "0 = Gray text & Strikethrough"
                    }
                  }
                ],
                "logic_OptionFieldGeneration": {
                  "trigger": "If 'OptionId' checkbox checked in Step 1 AND IsOptionField=1",
                  "action": "Add NEW row to DataGrid",
                  "fieldMapping": {
                    "SourceTableName": "[SourceTableName] + '_AOL'",
                    "SourceColumnName": "'OptionId'",
                    "DestColumnName": "[OriginalSourceColumnName] + 'OptionId'",
                    "ColumnSortNo": "[OriginalSortNo] + 0.1",
                    "IsOptionField": 1,
                    "FieldType": "'OptionId'",
                    "RefLevelNo": "From 'SQL - GetAOL' ([ColumnSortNo])",
                    "DataType": "'nvarchar'",
                    "MaxLength": 50,
                    "KeySortNo": 0,
                    "SourceKeyColumn": "[OriginalSourceColumnName]",
                    "DestKeyColumn": "[OriginalSourceColumnName] + 'OptionId'",
                    "AddToDest": 1
                  }
                }
              },
              "buttons": {
                "addReferenceColumn": {
                  "label": "Add Reference Column",
                  "logic": "Allows adding column based on mappings from Step 1 'Add Default Table Reference'"
                }
              }
            },
            "step4_Finish": {
              "action": "Commit to Database",
              "logic": {
                "1_CreateDBs": {
                  "SourceDB": "If not exists, create with [DBName]=@SourceDBName. Get ID.",
                  "DestDB": "If not exists, create with [DBName]=@DestDBName. Get ID."
                },
                "2_CreateTables": {
                  "SourceTable": "If not exists in [Setting].[Table], create.",
                  "DestTable": "If not exists in [Setting].[Table], create.",
                  "AOLTable": "If not exists, create virtual table [SourceTableName]+'_AOL'",
                  "RefTable": "If not exists, create (logic TBD)"
                },
                "3_CreateColumns": {
                  "SourceColumns": "If not exists in [Setting].[Column], create.",
                  "DestColumns": "If not exists in [Setting].[Column], create.",
                  "AOLColumns": "If not exists, create for virtual AOL table.",
                  "RefColumns": "If not exists, create (logic TBD)"
                },
                "4_Insert_TableDefine": {
                  "target": "[Setting].[TableDefine]",
                  "fields": {
                    "SourceMainDBId": "@SourceDBId",
                    "SourceMainDBName": "@SourceDBName",
                    "SourceMainSchemaName": "@SourceSchema",
                    "SourceMainTableId": "@SourceTableId",
                    "SourceMainTableName": "@SourceTable",
                    "DestMainDBId": "@DestDBId",
                    "DestMainDBName": "@DestDBName",
                    "DestMainSchemaName": "@DestSchema",
                    "DestMainTableId": "@DestTableId",
                    "DestMainTableName": "@DestTable",
                    "ModifiedBy": "ORIGINAL_LOGIN()"
                  }
                },
                "5_Insert_ReferenceTableDefine": {
                  "target": "[Setting].[ReferenceTableDefine]",
                  "entries": [
                    {
                      "type": "Main Relationship",
                      "fields": {
                        "TableDefineId": "@TableDefineId",
                        "VersionId": 1,
                        "RefTableName": "@SourceTable",
                        "RefTableId": "@SourceTableId",
                        "RefTableAliasName": "'SourceMainTable'",
                        "RefType": "'Main'",
                        "JoinType": "'INNER JOIN'",
                        "Description": "'SourceMainTable'"
                      }
                    },
                    {
                      "type": "OptionId Link",
                      "condition": "If OptionId records created",
                      "fields": {
                        "TableDefineId": "@TableDefineId",
                        "ParentReferenceTableDefineId": "@SourceReferenceTableDefineId",
                        "RefTableName": "@SourceTableName + '_AOL'",
                        "RefTableAliasName": "'RefOption_' + @OriganlSourceName",
                        "RefLevelNo": "From 'SQL - GetAOL' [ColumnSortNo]",
                        "RefType": "'OptionList'",
                        "JoinType": "'LEFT JOIN'",
                        "Description": "'RefOption_' + @OriganlSourceName..."
                      }
                    },
                    {
                      "type": "RefTable Link",
                      "condition": "From [DefaultTableReference]",
                      "fields": {
                        "TableDefineId": "@TableDefineId",
                        "RefTableName": "@SourceTableName + '_AOL'",
                        "RefTableAliasName": "'Ref' + [RefTableName] + '_' + [RefColumnName]",
                        "RefLevelNo": "DENSE_RANK() OVER(order by [RefTableId])",
                        "RefType": "'RefID'",
                        "JoinType": "'LEFT JOIN'"
                      }
                    }
                  ]
                },
                "6_Insert_ReferenceColumnDefine": {
                  "target": "[Setting].[ReferenceColumnDefine]",
                  "logic": "Create records linking Source/Dest/Ref columns (Main, OptionId, RefTable types) as per CSV logic."
                }
              }
            },
            "step5_Cancel": {
              "action": "Close Wizard, No changes applied"
            }
          }
        },
        {
          "id": "ViewDefine",
          "uiElements": {
            "topFilter": { "source": "ViewDefine" },
            "bottomButton": { "label": "Add new", "status": "Stub/TBD" }
          }
        },
        {
          "id": "IndexDefine",
          "uiElements": {
            "topFilter": { "source": "IndexDefine" },
            "bottomButton": { "label": "Add new", "status": "Stub/TBD" }
          }
        }
      ]
    },
    "rightPanel": {
      "description": "Contextual tabs for selected Left Panel item",
      "tabs": ["ColumnDefine", "ViewDefine", "IndexDefine"]
    },
    "menuBar": {
      "items": {
        "File": { "New": ["ColumnDefine", "ViewDefine", "IndexDefine"], "Exit": true },
        "Edit": { "Undo": true, "Redo": true, "Delete": true },
        "View": {
          "Toggles": ["ColumnDefine", "ViewDefine", "IndexDefine"],
          "Popups": [
            { "label": "Default Column BlackList", "target": "[dbo].[DefaultColumnBlackList]" },
            { "label": "Default Column Rename", "target": "[dbo].[DefaultColumnNameChange]" },
            { "label": "Default Table Reference", "target": "[dbo].[DefaultTableReference]" }
          ]
        },
        "Tools": {
          "Settings": [
            { "label": "Connections", "storage": "settings.json" },
            { "label": "Defaults option", "storage": "settings.json" },
            { "label": "Data Types", "storage": "settings.json" }
          ]
        },
        "Help": ["View Help", "About App"]
      }
    }
  }
}
```