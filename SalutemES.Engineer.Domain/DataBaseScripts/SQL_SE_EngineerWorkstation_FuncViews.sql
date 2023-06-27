Use DB_SE_EngineerWS;
Go

Create Function [dbo].GetComponentFilesCount(@Component_name string_short)
Returns int
As Begin
	Return (Select Count(*) From Component_File Where Component_File.component = @Component_name)
End
Go

Create View [dbo].VFamilyList As
Select		Product.family,
			Count(*) as 'count'
From		Product
Group by	Product.family;
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
Inner join	Structure on Structure.component = Component.name
Go

Create View [dbo].VComponentUsageList As
Select		Component.name,
			Component.code,
			[dbo].GetComponentFilesCount(Component.name) as 'files_count',
			Count(Structure.product) as 'used_count'
From		Component
Left join	Structure on Structure.component = Component.name
Group by	Component.name, Component.code;
Go