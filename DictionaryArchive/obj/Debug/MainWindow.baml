   M S B A M L   `   `   `  ����L  HDictionaryArchive, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:DictionaryArchiveDictionaryArchive  5   	   "    DictionaryArchive.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   
   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   1localclr-namespace:DictionaryArchive     5   	     � Title$  
MainWindow��5   	   $��749.719��6   $��742.268��6-   .��5	      � $��0,0,0,-2q�6    � RowDefinitions 5
   
   �� 5      $
��313* �6   �� 5      $
��355* �6    � ColumnDefinitions 5   
   �� 5      $	��93* �6   �� 5      $
��381* �6   �� 5      $
��277* �6   .+�5   
   �� -   5         ��Name$ SourceString��5      $
��Left=�6(   $	��187��6C   $��
10,124,0,0q�6P    �� TextWrapping$
 Wrap=�6d   $�� ��6x   $	��Top=�6�   $	��443��6�   $��2��6�   �� 5   
   -   5        $ WordDictionary��5      $��2��6*   $	��190��6:   $��	0,121,9,0q�6G   $
 Wrap=�6Z   $�� ��6n   $	��Top=�6v   �� 5   
   -   5         ��Name$ DictionarySize��5      $��Dictionary size: ��6,   $
��Left=�6E   $
��Wrap=�6`   $	��Top=�6t   $��2��6�   $��	0,100,0,0q�6�   $	��123��6�   �� 5   
   -   5        $ 	WordCount��5      $��File's context��6'   $
��Left=�6=   $
��Wrap=�6X   $	��Top=�6l   $��
10,100,0,0q�6�   $	��127��6�   $��2��6�   �� 5   
   -   5        $ 
ResultForm��5      $	��365��6&   $��10,0,9,0q�63   $
 Wrap=�6E   $�� ��6Y   $	��Top=�6a   $��3��6y   $��1��6�   �� 5   
   -   5        $ ResultTextTitle��5      $��Result��6-   $
��Left=�6;   $
��Wrap=�6V   $	��Top=�6j   $��
10,312,0,0q�6�   $	��127��6�   $��2��6�   (� 5   
   $��1��6   $��93,6,131,248q�6"    (� Stretch$
 Fill=�68   .��5      � $��84��6   $	��148��6   .+�5      �� -   5         ��Name$ 
Decompress��5      $��
Decompress��6/   $
��Left=�6C   $	��Top=�6^   $��84��6v   $	��148��6�   �� 5      -   5         ��Name$ Decode��5      $��Decode��6)   $
��Left=�6:   $��	10,54,0,0q�6U   $	��Top=�6h   $	��126��6�   6�   �� 5      -	   5        $ openEncodeFile��5      $��Open encode file��61   $
��Left=�6L   $��	10,24,0,0q�6g   $	��Top=�6z   $	��126��6�   6�   5       5!      (� 5"   
   $��2��6   $��14,6,298,242q�6&   $
 Fill=�6<   .��5#      � $��84��6   $	��151��6   .+�5$      �� -
   5        $ 	Compress1��5$      $��Compress��6.   $
��Left=�6@   $	��Top=�6[   	 J� RenderTransformOrigin$	 0.5,0.5&�6s   $��80��6�   $	��151��6�   �� 5&      -   5        $ OpenFile��5&      $��Open source file��6+   $
��Left=�6F   $��	10,24,0,0q�6a   $	��Top=�6t   $	��127��6�   6�   �� 5'      -   5        $ Compress��5'      $��Compress��6+   $
��Left=�6>   $��	10,54,0,0q�6Y   $	��Top=�6l   $	��127��6�   6�   5)      5*      5,      5-      