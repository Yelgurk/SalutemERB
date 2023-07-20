Use DB_SE_EngineerWS;
Go

Create Procedure [dbo].[GetExportTable]
	@ProductsList export_product readonly
As Begin
	Select  Component.name,
		Component.code,
		sum(Structure.count * TT.count) as 'count',
		Component.grade,
		Component.thickness,
		Replace(Component.folds, 0, '') as 'folds',
		Component.weightKG,
		sum(Component.weightKG * Structure.count * TT.count) as 'totalKG',
		Component.note,
		Component.material
	From Component
	Inner Join Structure on Structure.component = Component.name
	Inner Join @ProductsList as TT on TT.product = Structure.product
	Group by	Component.name,
			Component.code,
			Component.grade,
			Component.thickness,
			Component.folds,
			Component.weightKG,
			Component.note,
			Component.material;
End
Go

Create procedure [dbo].GetProductsListByComponent
	@Component string_short
As Begin
	Select	VProductList.*
	From	VProductList
	Inner Join VProductComponentsList on VProductComponentsList.product = VProductList.name
	Where	VProductComponentsList.name = @Component
End
Go

Create Procedure [dbo].GetProductsListByFamily
	@Family string_short
As Begin
	Select	*
	From	VProductList
	Where	VProductList.family = @Family
End
Go

Create Procedure [dbo].GetComponentsListByProduct
	@Product string_short
As Begin
	Select	*
	From	VProductComponentsList
	Where	VProductComponentsList.product = @Product
End
Go

Create Procedure [dbo].GetComponentsDetails
	@Name string_short,
	@Code string_short
As Begin
	Select	'' as 'doesntMatter',
		    Component.*,
			[dbo].GetComponentFilesCount(Component.name) as 'files_count'
	From	Component
	Where	Component.name = @Name And
			Component.code = @Code
End
Go

Create Procedure [dbo].GetFilesListByComponent
	@Component string_short
As Begin
	Select	*
	From	Component_File
	Where	Component_File.component = @Component
End
Go

Create Procedure [dbo].RenameFamily
	@Fiamily string_short,
	@NewName string_short
As Begin
	Update	Family
	Set		Family.name = @NewName
	Where	Family.name = @Fiamily
End
Go

Create Procedure [dbo].RenameProduct
	@Product string_short,
	@NewName string_short
As Begin
	Update	Product
	Set		Product.name = @NewName
	Where	Product.name = @Product
End
Go

Create Procedure [dbo].RenameComponent
	@Component string_short,
	@NewName string_short
As Begin
	Update	Component
	Set		Component.name = @NewName
	Where	Component.name = @Component
End
Go

Create Procedure [dbo].AddComponentFile
	@Component string_short,
	@FilePath string_long
As Begin
	If not exists (Select *
					From	Component_File
					Where   Component_File.component = @Component And
							Component_File.localFilePath = @FilePath)
	Begin
		Insert Into Component_File Values
		(@Component, @FilePath);
	End
End
Go

Create Procedure [dbo].AddComponent
	@Name string_short,
	@Code string_short,
	@Grade string_short,
	@Thickness decimal(3, 1),
	@Folds int,
	@WeightKG decimal(5, 2),
	@Note string_medium,
	@Material string_short
As Begin
	Insert Into Component Values
	(@Name, @Code, @Grade, @Thickness, @Folds, @WeightKG, @Note, @Material);
End
Go

Create Procedure [dbo].AddProduct
	@Name string_short,
	@Family string_short
As Begin
	Insert Into Product Values
	(@name, @Family);
End
Go

Create Procedure [dbo].AddFamily
	@Name string_short
As Begin
	Insert Into Family Values
	(@Name);
End
Go

Create Procedure [dbo].AddProductComponent
	@Product string_short,
	@Component string_short,
	@Count int
As Begin
	If not exists (Select *
					From	Structure
					Where   Structure.product = @Product And
							Structure.component = @Component)
	Begin
		Insert Into Structure Values
		(@Product, @Component, @Count);
	End
	Else
	Begin
		Update	Structure
		Set		Structure.count = @Count
		Where	Structure.product = @Product And
				Structure.component = @Component;
	End
End
Go

Create Procedure [dbo].DeleteComponentFile
	@Component string_short,
	@FilePath string_long
As Begin
	Delete From Component_File
	Where	Component_File.component = @Component And
			Component_File.localFilePath = @FilePath;
End
Go

Create Procedure [dbo].DeleteComponent
	@Name string_short,
	@Code string_short
As Begin
	Delete From Component
	Where	Component.name = @Name And
			Component.code = @Code;
End
Go

Create Procedure [dbo].DeleteProduct
	@Name string_short
As Begin
	Delete From Product
	Where Product.name = @Name;
End
Go

Create Procedure [dbo].DeleteFamily
	@Name string_short
As Begin
	Delete From Family
	Where Family.name = @Name;
End
Go

Create Procedure [dbo].DeleteProductComponent
	@Component string_short,
	@Product string_short
As Begin
	Delete From Structure
	Where	Structure.component = @Component And
			Structure.product = @Product;
End
Go

Create Procedure [dbo].EditComponent
	@Name string_short,
	@Code string_short,
	@NewName string_short,
	@NewCode string_short,
	@Grade string_short,
	@Thickness decimal(3, 1),
	@Folds int,
	@WeightKG decimal(5, 2),
	@Note string_medium,
	@Material string_short
As Begin
	Update	Component
	Set		Component.name = @NewName,
			Component.code = @NewCode,
			Component.grade = @Grade,
			Component.thickness = @Thickness,
			Component.folds = @Folds,
			Component.weightKG = @WeightKG,
			Component.note = @Note,
			Component.material = @Material
	Where	Component.name = @Name And
			Component.code = @Code
End
Go

Create Procedure [dbo].IsComponentExists
	@Name string_short,
	@Code string_short
As Begin
	Declare @Counter int = 0;

	If ((Select Count(*) From Component Where Component.name = @Name) > 0) Begin
		Set @Counter = @Counter + 1;
	End
	
	If ((Select Count(*) From Component Where Component.code = @Code) > 0) Begin
		Set @Counter = @Counter + 2;
	End

	Select @Counter;
End
Go