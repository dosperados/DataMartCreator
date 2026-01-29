#DMCApp 
# Логика работы приложения (отступами задана структура)
1. Левая панель (Вкладки как в Visual Studio, например: "Solution Explorer", "Properties"):
	1. TableDefine
		1. С верху есть поле фильтрации, которое фильтрует отображаемый список имён таблиц из [Setting].[TableDefine]. (отображается одни элемент, значения поля [DestMainTableName] из [Setting].[TableDefine] при этом фильтрации происходит тоже по этому полю)
		2. Снизу кнопка "Add new" которая запускает Wizard 
			1. Первое окно Wizard`a
				1. Стрелка назад, что бы была возможность вернутся на предыдущий шаг Wizard`a
				2. Заголовок: "Add new TableDefine", информационная строка: Step 1: Source and Destination scope, более мелким шрифтом описание "Specify the Source and Destination Tables."
				3. Отображаемые поля:  (Все эти поля с ограничением ввода до 128 символов, каждое поле): 
					1. Source DB (выбирают из настроек, а также доступно для ручного ввода текста)
						1. Можно выбрать только из тех DB, что добавлены в Settings как Source DB
					2. Source Schema (выбирают из настроек, а также доступно для ручного ввода текста)
						1. Зная Source DB мы можем обратиться к этой DB и запросить список Schema, предоставив их к выбору. (SQL - GetSchema)
					3. Source Table (выбирают из настроек, а также доступно для ручного ввода текста_ SQL - GetTableNames)
						1. Зная Source DB и Schema можно запросить список таблиц из источника по указанной схеме, плюс тут должна быть возможность поставить фильтр, так как список таблиц может быть очень большой.  (Или альтернативно можно ввести название таблице простым вводом текста с клавиатуры)
					4. Dest DB
						1. Ввод с клавиатуры (по умолчанию вставить TADM)
					5. Dest Schema 
						1. Ввод с клавиатуры (по умолчанию вставить dbo)
					6. Dest Table
						1. Ввод с клавиатуры (по умолчанию вставить dim)
					7. Checkboxes (каждый из Checkbox включает добавление соответствующих строк в результирующий набор данных, по умолчанию эти Checkbox включены):
						1. Add "OptionId" Column (добавляет соответсвующие записи в [ReferenceTableDefine],[ReferenceColumnDefine], [ColumnDefine] которые реализовывают связку с "AOL" )
						2. Add "@CompanyId"  (добавляет помимо стандартной связки по суррогатным ключам, связку по @CompanyId в [TADM].[Setting].[ReferenceColumnDefine])
					8. Кнопка "Add Default Table Reference" 
						1. Открывает всплывающее окно редактирования таблицы [dbo].[DefaultTableReference] (по сути задает возможность добавить дополнительную связку к другим таблицам, все из полей могут быть доступны как для ручного ввода так и выбора из выпадающего меню SourceDB или DestDB)
							1. Пример 1: 'TADWH' AS [SourceDBName]
								,'dbo' AS [SourceSchemaName]
								,'ServiceInvoiceLine' AS [SourceTableName]
								,'CustomerNo' AS [SourceColumnName]
								,'TADM' AS [DestDBName]
								,'dbo' as [DestSchemaName]
								,'dimServiceInvoiceLine' AS [DestTableName]
								,'SelltoCustomerNo' AS [DestColumnName]
								,'TADWH' AS [RefDBName]
								,'dbo' AS [RefSchemaName]
								,'Customer' AS [RefTableName]
								,'No' as [RefColumnName]
								,'CustomerId' as [RefColumnIdName]
								,'SelltoCustomerId' AS [AddRefDestColumnIdName] 
							2. Пример 2: 'TADWH' AS [SourceDBName]
								,'dbo' AS [SourceSchemaName]
								,'ServiceInvoiceLine' AS [SourceTableName]
								,'BilltoCustomerNo' AS [SourceColumnName]
								,'TADM' AS [DestDBName]
								,'dbo' as [DestSchemaName]
								,'dimServiceInvoiceLine' AS [DestTableName]
								,'BilltoCustomerNo' AS [DestColumnName]
								,'TADWH' AS [RefDBName]
								,'dbo' AS [RefSchemaName]
								,'Customer' AS [RefTableName]
								,'No' as [RefColumnName]
								,'CustomerId' as [RefColumnIdName]
								,'BilltoCustomerId' AS [AddRefDestColumnIdName]
							3. Пример 3: 'TADWH' AS [SourceDBName]
								,'dbo' AS [SourceSchemaName]
								,'ServiceInvoiceLine' AS [SourceTableName]
								,'ItemCategoryCode' AS [SourceColumnName]
								,'TADM' AS [DestDBName]
								,'dbo' as [DestSchemaName]
								,'dimServiceInvoiceLine' AS [DestTableName]
								,'ItemCategoryCode' AS [DestColumnName]
								,'TADWH' AS [RefDBName]
								,'dbo' AS [RefSchemaName]
								,'ItemCategory' AS [RefTableName]
								,'Code' as [RefColumnName]
								,'ItemCategoryId' as [RefColumnIdName]
								,'ItemCategoryId' AS [AddRefDestColumnIdName]
							4. Пример 4: 'TADWH' AS [SourceDBName]
								,'dbo' AS [SourceSchemaName]
								,'ServiceInvoiceLine' AS [SourceTableName]
								,'ServiceItemNo' AS [SourceColumnName]
								,'TADM' AS [DestDBName]
								,'dbo' as [DestSchemaName]
								,'dimServiceInvoiceLine' AS [DestTableName]
								,'ServiceItemNo' AS [DestColumnName]
								,'TADWH' AS [RefDBName]
								,'dbo' AS [RefSchemaName]
								,'ServiceItem' AS [RefTableName]
								,'No' as [RefColumnName]
								,'ServiceItemId' as [RefColumnIdName]
								,'ServiceItemId' AS [AddDestRefColumnIdName]
					9. В Bottom кнопки:
						1. "Next" (не становится активной пока не будут заполнены поля: Source DB, Source Schema, Source Table, Dest DB, Dest Schema, Dest Table)
						2. "Cancel"
			2. Второе окно Wizard`a
				1. Должны быть возможность вернутся на предыдущий шаг Wizard`a
				2. Заголовок: "Add new TableDefine", информационная строка: "Step 2: Column Definitions", более мелким шрифтом описание "Review columns. Blacklisted items are gray/strikethrough but can be enabled via 'AddToDest'."
				3. Ниже отображается DataGrid:
					1. Формирование DataGrid
						1. Получение информации о колонках источника с помощью SQL - "GetSourceColumn"
							1. Список элементов:
								1. [TableDefineId] (скрытое поле, в интерфейсе не отображается)
								2. [ReferenceTableDefineId] (скрытое поле, в интерфейсе не отображается)
								3. [SourceDBId] (скрытое поле, в интерфейсе не отображается)
								4. [SourceDBName] (скрытое поле, в интерфейсе не отображается)
								5. [SourceTableId] (скрытое поле, в интерфейсе не отображается)
								6. [SourceTableName] (скрытое поле, в интерфейсе не отображается)
								7. [SourceColumnId] (скрытое поле, в интерфейсе не отображается)
								8. Source Column -> [SourceColumnName] (not editable)
								9. Custome Statment -> [CustomeStatment] (editable)
								10. [DestDBId] (скрытое поле, в интерфейсе не отображается)
								11. [DestDBName] (скрытое поле, в интерфейсе не отображается)
								12. [DestTableId] (скрытое поле, в интерфейсе не отображается)
								13. [DestTableName] (скрытое поле, в интерфейсе не отображается)
								14. [DestColumnId] (скрытое поле, в интерфейсе не отображается)
								15. Dest column -> [DestColumnName] (editable)
									1. если находится соответствие полей в таблице [Setting].[DefaultColumnNameChange] (должны совпасить [SourceTableName], [SourceColumnName], [DestTableName] ) то используется значение поля [DestColumnName]
									2. Если название поля начинается в заглавных букв "TA", то нужно убрать (вырезать эти буквы) и оставить оставшийся текст Например "TAOrderNo" -> "OrderNo". (Если при этом новое имя уже есть как [SourceColumnName] то его пометить как [AddToDest] = 0). Не менять поле даже если она начинается с двух заглавных букв "TA" если [IsOptionField] = 1 т.е. если это поле совпадает с одним из значений поля [FieldNameDWH] в запросе "SQL - GetAOL" 
									3. Поля начинающиеся с "Orig"
								16. Column Sort -> [ColumnSortNo]  (editable)
								17. Is Option Field -> [IsOptionField] (checkbox)
									1. поле является [IsOptionField] (устанавливается в 1) если поле [FieldNameDWH] из запроса "SQL - GetAOL" находится такое же поле [SourceColumnName] Например в [SourceColumnName] мы имеем 'AppliestoDocType' и  одно из значений [FieldNameDWH] тоже равно 'AppliestoDocType', тогда это поле является [IsOptionField]=1
								18. Field Type -> [FieldType] (editable) (На текущий момент могут быть значения: OptionNo, OptionId, OptionDE, OptionENU, OptionHashId, RefId, NULL, но умолчанию NULL, устанавливается при добавлении новых элементов автоматически или вручную в интерфейсе)
								19. Data Type -> [DataType] (editable) (настройки DataType сохраняются в файле settings.json) 
									1. Некоторые типы данных заменяются автоматически, например WHERE [SourceColumnName] like '%Date'  AND c.[DataType] = 'datetime' THEN [DataType] = 'DATE'
									2. WHEN [DataType] = 'decimal' AND [Precision] = 38 THEN  [Precision] = 18 ,[Scale] = 2
									3. Если пользователь хочет отредактировать это поле, то он может выбрать значение из выпадающего списка (содержащий следующие значения: int, decimal, VARBINARY, datetime, uniqueidentifier, tinyint, DATE, char, bigint, nvarchar, bit  - этот список хранится в settings.json)
										1. при этом должна быть валидация:
											1. WHEN [DataType] IN ("VARBINARY", "nvarchar", "char") THEN [MaxLength] = 1 , [Precision] = NULL, [Scale] = NULL, [CollationName] = "Latin1_General_CI_AS"
											2. WHEN [DataType] = "tinyint" THEN [MaxLength] = NULL, [Precision] = 3, [Scale] = 0, [CollationName] = NULL
											3. WHEN [DataType] = "int" THEN [MaxLength] = NULL, [Precision] = 10, [Scale] = 0, [CollationName] = NULL
											4. WHEN [DataType] = "bigint" THEN [MaxLength] = NULL, [Precision] = 19, [Scale] = 0, [CollationName] = NULL
											5. WHEN [DataType] = "decimal" THEN [MaxLength] = NULL, [Precision] = 18, [Scale] = 2, [CollationName] = NULL
											6. WHEN [DataType] = "DATE" THEN [MaxLength] = NULL, [Precision] = NULL, [Scale] = NULL, [CollationName] = NULL
											7. WHEN [DataType] = "uniqueidentifier" THEN [MaxLength] = NULL, [Precision] = NULL, [Scale] = NULL, [CollationName] = NULL
								20. Max Length -> [MaxLength] (editable)
								21. Precision -> [Precision] (editable)
								22. Scale -> [Scale] (editable)
								23. Is Nullable -> [IsNullable] (checkbox) (editable)
								24. Collation -> [CollationName] (editable) (выбор из выпадающего списка, список хранится в settings.json, на сейчас достпуно одно значение "Latin1_General_CI_AS")
								25. Default Value -> [DefaultValue] (editable)
								26. Key Sort No -> [KeySortNo] (editable)
								27. Source Key Column -> [SourceKeyColumn] (editable)
								28. Dest Key Column -> [DestKeyColumn] (editable)
								29. Ref Table -> [RefTableName] (editable) (по умолчанию NULL, устанавливается в значение при добавлении нового элемента либо автоматически или вручную)
								30. Ref Column  -> [RefColumnName] (editable)
								31. Ref Level No -> [RefLevelNo] (editable) (по умолчанию NULL, устанавливается в значение при добавлении нового элемента либо автоматически или вручную)
								32. Add - > [AddToDest] (editable) (checkbox)
									1. Этот checkbox определяет будет ли этот элемент добавлен в результирующий набор для вставки в DB или нет. 
									2. Логика - Все поля устанавливаются в значение 1 за исключением тех, которые есть в [dbo].[DefaultColumnBlackList].
									3. Поля установленные в 0 отображаются серым цветом и вся строка перечеркнута. 
									4. При изменении этого checkbox пользователем убирает или добавляет перечеркивание и серый цвет. (то есть включает или выключает элемент) где 1 значит это поле будет добавлено в результирующий набор, а 0 значит это поле не будет добавлено. 
						2. Добавления новых OptionField элементов (каждое поле помеченное как [IsOptionField] обрабатывается этой логикой)
							1. Добавление OptionId (если на шаге 1 Wizard была включена эта опция)
								1. Автоматически добавляется новый элемент в этот DataGrid (набор данных)
									1. Поля которые я не упомянул остаются такими же как у поля источника для каждого [IsOptionField] в этом наборе
									2. [ReferenceTableDefineId] (скрытое поле, в интерфейсе не отображается) 
									3. [SourceTableId] (скрытое поле, в интерфейсе не отображается) 
									4. [SourceTableName] (скрытое поле, в интерфейсе не отображается) Виртуальная таблица (запрос "SQL - GetAOL"), которая создается при определении [IsOptionField] ее имя формируется так: [SourceTableName]+"`_AOL`" Например: "SalesInvoiceHeader_AOL"  
									5. [SourceColumnId] (скрытое поле, в интерфейсе не отображается)
									6. [SourceColumnName] = "OptionId"
									7. [DestColumnName] = original value [SourceColumnName]+"OptionId" (example: WHEN [IsOptionField] = 1 AND [SourceColumnName] = "AppliestoDocType" THEN [DestColumnName] = "AppliestoDocTypeOptionId")
									8. [ColumnSortNo] = [ColumnSortNo] источника + 0,1 например: [ColumnSortNo]= 89 THEN 89,1
									9. [IsOptionField] = 1
									10. [FieldType] = "OptionId"
									11. [RefLevelNo] = из запроса "SQL - GetAOL" значение поля [ColumnSortNo]
									12. [DataType] = "nvarchar"
									13. [MaxLength] = 50
									14. [Precision] = NULL, [Scale] = NULL, [CollationName] = "Latin1_General_CI_AS"
									15. [DefaultValue] = NULL
									16. [KeySortNo] = 0
									17. [SourceKeyColumn] = original value [SourceColumnName]
									18. [DestKeyColumn] = original value [SourceColumnName]+"OptionId"
									19. [AddToDest] = 1
			3. Кнопка "Add Reference Column"
				1. На основании заданной связки в предидущем шаге "Add Default Table Reference" позволяет добавить новую Column в результирующий набор.
			4. Нажатие кнопки "Finish" в Wizard ( собранные данные (DTO) передаются в соответствующий таблицы)
				1. Создание записей DB 
					1. Source DB ( если DB с таким именем нет то создается запись [DBName] = @SourceDBName, если уже создана, то получается соответствующий [DBId] FROM [Setting].[DB] WHERE [DBName] = @SourceDBName)
					2. Dest DB (если DB с таким именем нет то создается запись [DBName] = @DestDBName, если уже создана, то получается соответствующий [DBId] FROM [Setting].[DB] WHERE [DBName] = @DestDBName)
				2. Создание записей в Tables
					1. Source Table (Если в [Setting].[Table] еще не создана таблица с источника [DBId] = @SourceDBId  AND [SchemaName] = @SourceSchema AND [TableName] = @SourceTable )
					2. Dest Table (Если в [Setting].[Table] еще не создана таблица с источника [DBId] = @DestDBId  AND [SchemaName] = @DestSchema AND [TableName] = @DestTable )
					3. AOL Table (Если в [Setting].[Table] еще не создана виртуальная таблица [SourceTableName]+"`_AOL`")
					4. Ref Table (Если в [Setting].[Table] еще не создана reference таблица, то создается - про этот тип таблиц я еще не описывал в логике и буду расписывать этот функционал позже)
				3. Создание Записей в Columns
					1. Source Column (Если в [Setting].[Column] еще не созданы Column из Source Table то создаются, чтобы потом можно было связать и получать Id для вставки данных в Setting.TableDefine, Setting.ReferenceTableDefine, Setting.ReferenceColumnDefine, Setting.ColumnDefine)
					2. Dest Column (Если в [Setting].[Column] еще не созданы Column из Dest Table то создаются, чтобы потом можно было связать и получать Id для вставки данных в Setting.TableDefine, Setting.ReferenceTableDefine, Setting.ReferenceColumnDefine, Setting.ColumnDefine)
					3. AOL Column (Если в [Setting].[Column] еще не созданы Column из виртуальной таблицы SQL - GetAOL то создаются новые записи, чтобы потом можно было связать и получать Id для вставки данных в Setting.TableDefine, Setting.ReferenceTableDefine, Setting.ReferenceColumnDefine, Setting.ColumnDefine)
					4. Ref Column (Если в [Setting].[Column] еще не создана reference Column, то создается - про этот тип я еще не описывал в логике и буду расписывать этот функционал позже)
				4. Создание записи в [Setting].[TableDefine] (эта таблица определяет связку SourceTable and DestTable)
					1. [Setting].[TableDefine] ([SourceMainDBId] = @SourceDBId, [SourceMainDBName] = @SourceDBName, [SourceMainSchemaName] = @SourceSchema, [SourceMainTableId] = @SourceTableId, [SourceMainTableName] = @SourceTable, [DestMainDBId] =@DestDBId, [DestMainDBName]=@DestDBName, [DestMainSchemaName] = @DestSchema, [DestMainTableId] = @DestTableId, [DestMainTableName] = @DestTable, [ModifiedBy] = ORIGINAL_LOGIN())
				5. Создание записей в [Setting].[ReferenceTableDefine] 
					1. определение главной связкаи  Source and Dest полей
						1. [TableDefineId] = @TableDefineId, [VersionId] = 1, [RefTableName] = @SourceTable, [RefTableId] = SourceTableId, [RefTableAliasName] = 'SourceMainTable', [CustomeTableSelect] = NULL, [RefLevelNo] = 0, [RefType] = 'Main', [JoinType] = 'INNER JOIN', [Description] = 'SourceMainTable'
					2. связки RefTable
						1. Связка полей для OptionId (Если были созданы записи OptionId для них должны быть создана запись)
							1. [TableDefineId] =@TableDefineId, [VersionId] = 1, [ParentReferenceTableDefineId] = @SourceReferenceTableDefineId, [RefTableName] =@SourceTableName+"`_AOL`", [RefTableId] = @RefTableId, [RefTableAliasName] = "RefOption_"+@OriganlSourceName, [CustomeTableSelect] = NULL, [RefLevelNo] = из запроса "SQL - GetAOL" значение поля [ColumnSortNo] , [RefType] = "OptionList", [JoinType] = "LEFT JOIN", [Description]= "RefOption_"+@OriganlSourceName+(из запроса "SQL - GetAOL" значение поля [ColumnSortNo])
						2. Связка для добавленных RefTable через [dbo].[DefaultTableReference] (большая часть полей берется как раз из этой таблицы [DefaultTableReference])
							1. [TableDefineId] =@TableDefineId, [VersionId] = 1, [ParentReferenceTableDefineId] = @SourceReferenceTableDefineId, [RefTableName] =@SourceTableName+"`_AOL`", [RefTableId] = @RefTableId, [RefTableAliasName] = 'Ref'+a.[RefTableName]+'`_`'+[RefColumnName], [CustomeTableSelect] = NULL, [RefLevelNo] =  CAST(DENSE_RANK() OVER(order by [RefTableId]) AS NVARCHAR(3)), [RefType] = "RefID", [JoinType] = "LEFT JOIN", [Description]= 'Ref'+[RefTableName]+'`_`'+a.[RefColumnName]+CAST(DENSE_RANK() OVER(order by [RefTableId]) AS NVARCHAR(3))
				6. Создание записей в [Setting].[ReferenceColumnDefine] (предоставлю пример в CSV)
					1. для главной связки (описывает связь таблицы источника и таблицы назначения)
						1. В таблице создается запись в которой [SourceReferenceTableDefineId] = [RefReferenceTableDefineId] 
					2. для OptionId связки
						1. Создаются соответвующие связи 
					3. для RefTable связки
						1. Создаются соответвующие связи 
			5. Нажатие кнопки "Cancel" в Wizard (ничего не применяется окно Wizard закрывается)
	2. ViewDefine
		1. С верху есть поле фильтрации, которое фильтрует отображаемый список имён таблиц из ViewDefine. Поле отображается и фильтрации ViewDefine
		2. Снизу кнопка "Add new" которая запускает Wizard 
			1. пока сделать только заглушку - логика будет описана позже
	3. IndexDefine
		1. С верху есть поле фильтрации, которое фильтрует отображаемый список имён таблиц из IndexDefine. Поле отображается и фильтрации IndexDefine
		2. Снизу кнопка "Add new" которая запускает Wizard 
			1. пока сделать только заглушку - логика будет описана позже
2. Правая панель: 
	1. Всегда содержит связную информацию по выбранной записи из левой вкладки
	2. Представляет из себя набор вкладок, каждая из которых отображает связные данные из соответствующих таблиц:
		1. ColumnDefine
		2. ViewDefine 
		3. IndexDefine 
3. В верхнем меню (MyMenuBar):
	1. File
		1. New
			1. ColumnDefine (Запускает соответствующий визард)
			2. ViewDefine (Запускает соответствующий визард)
			3. IndexDefine (Запускает соответствующий визард)
		2. Exit
	2. Edit
		1. Undo
		2. Redo
		3. Delete
	3. View
		1. ColumnDefine (показать/скрыть)
		2. ViewDefine (показать/скрыть)
		3. IndexDefine (показать/скрыть)
		4. разделитель ---
		5. Default Column BlackList
			1. Открывает всплывающее окно редактирования таблицы [dbo].[DefaultColumnBlackList]
		6. Default Column Rename
			1. Открывает всплывающее окно редактирования таблицы [dbo].[DefaultColumnNameChange]
		7. Default Table Reference 
			1. Открывает всплывающее окно редактирования таблицы [dbo].[DefaultTableReference]
	4. Tools
		1. Settings 
			1. Connections (открывает всплывающее окно настройки подключения к базам данных Source и Dest, настройки подключения к БД сохраняются в файле settings.json)
			2. Defaults option (открывает всплывающее окно настройки какие опции Default включены по умолчанию а какие отключены, настройки сохраняются в файле settings.json)
			3. Data Types (открывает всплывающее окно настройки добавления или удаления [DataType],  настройки DataType сохраняются в файле settings.json)
	5. Help
		1. View Help
		2. About App

