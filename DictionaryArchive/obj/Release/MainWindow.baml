   M S B A M L   `   `   `  ���� L  HDictionaryArchive, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:DictionaryArchiveDictionaryArchive  "    DictionaryArchive.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     1localclr-namespace:DictionaryArchive       � Title$  
MainWindow��$��749.719��$��742.268��.��� $��0,0,0,-2q� � RowDefinitions �� $
��313* ��� $
��355* � � ColumnDefinitions �� $	��93* ��� $
��381* ��� $
��277* �.+��� -    ��Name$ OpenFile��$��Open source file��$
��Left=�$��	10,20,0,0q�$	��Top=�$��77���� -    ��Name$ SourceString��$
��Left=�$	��352��$��	10,76,0,0q� �� TextWrapping$
 Wrap=�$�� ��$	��Top=�$	��421��$��2��$��2���� -   $ WordDictionary��$��2��$	��352��$��0,73,9,0q�$
 Wrap=�$�� ��$	��Top=�$��2���� -    ��Name$ DictionarySize��$��Dictionary size: ��$
��Left=�$
��Wrap=�$	��Top=�$��2��$��0,37,0,0q�$	��123���� -   $ 	WordCount��$��Count of words: ��$
��Left=�$
��Wrap=�$	��Top=�$��	10,52,0,0q�$	��127��$��2���� -   $ SaveDictionary��$
��Save��$��2��$
��Left=�$��
136,35,0,0q�$	��Top=�$	��125���� -   $ 
ResultForm��$	��230��$��
10,135,9,0q�$
 Wrap=�$�� ��$	��Top=�$��3��$��1���� -   $ ResultTextTitle��$��Result��$
��Left=�$
��Wrap=�$	��Top=�$��
10,110,0,0q�$	��127��$��2��$��1���� -	   $ 
SaveResult��$
��Save��$��1��$
��Left=�$��
60,106,0,0q�$	��Top=�$	��125��$��1���� -
   $ LengthResult��$��Length: ��$
��Left=�$
��Wrap=�$	��Top=�$��245,108,0,0q�$	��127��$��1��$��1���� -   $ LengthSorce��$��Length: ��$
��Left=�$
��Wrap=�$	��Top=�$��
113,52,0,0q�$	��126��$��1���� -   $ Encode��$��Encode��$��1��$
��Left=�$��
162,20,0,0q�$	��Top=�$��75���� -   $ Decode��$��Decode��$��1��$
��Left=�$��
265,20,0,0q�$	��Top=�$��75���� -   $ OpenDecodeFile��$��Open encode file��$
��Left=�$��	28,20,0,0q�$	��Top=�$��77��$��1��