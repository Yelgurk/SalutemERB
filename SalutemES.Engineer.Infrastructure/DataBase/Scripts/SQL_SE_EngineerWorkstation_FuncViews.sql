Use DB_SE_EngineerWS;
Go

Create Function [dbo].GetComponentFilesCount(@Component_code string_short)
Returns int
As Begin
	Return (Select Count(*) From Component_File Where Component_File.component = @Component_code)
End
Go

Create View [dbo].VFamilyList As
Select		Family.name,
			Count(Product.family) as 'count'
From		Product
Right Join	Family on Family.name = Product.family
Group by	Family.name;
Go

Create View [dbo].VProductList As
Select		Product.*,
			Count(Structure.component) as 'component_count'
From		Product
Left join	Structure on Structure.product = Product.name
Group by	Product.name, Product.family;
Go

Create View [dbo].VProductComponentsList As
Select		Structure.product,
			Component.*,
			[dbo].GetComponentFilesCount(Component.name) as 'files_count'
From		Component
Inner join	Structure on Structure.component = Component.code
Go

Create View [dbo].VComponentUsageList As
Select		Component.name,
			Component.code,
			[dbo].GetComponentFilesCount(Component.code) as 'files_count',
			Count(Structure.product) as 'used_count'
From		Component
Left join	Structure on Structure.component = Component.code
Group by	Component.name, Component.code;
Go

Create View [dbo].VComponentListFullInfo As 
	Select	Component.name,
			Component.code,
			(Select '1') as 'count',
			Component.grade,
			Component.thickness,
			Replace(Component.folds, 0, '') as 'folds',
			Component.weightKG,
			IsNull(sum(Component.weightKG * Structure.count), 0) as 'totalKG',
			Component.note,
			Component.material
	From	Component
	Left Join	Structure on Structure.component = Component.code
	Group by	Component.code,
				Component.name,
				Component.grade,
				Component.thickness,
				Component.folds,
				Component.weightKG,
				Component.note,
				Component.material;
Go