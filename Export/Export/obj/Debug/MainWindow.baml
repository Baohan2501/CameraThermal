   M S B A M L   `   `   `  ����?  ;Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3%clr-namespace:Core.Base;assembly=Core	Core.Base  5   	   H DThermalCamera, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:Export.ViewModelExport.ViewModel 5   	   3clr-namespace:Export.ConverterExport.Converter 5	   	   ;)clr-namespace:Core.Behavior;assembly=CoreCore.Behavior  5
   	   c _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35qPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.InteractivitySystem.Windows.Interactivity 5   	   )clr-namespace:Export.ViewExport.View 5   	   clr-namespace:ExportExport 5   	      Export.MainWindow   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   4wb%clr-namespace:Core.Base;assembly=Core     5   	   H DMahApps.Metro, Version=1.6.5.1, Culture=neutral, PublicKeyToken=null@mahAppControl,http://metro.mahapps.com/winfx/xaml/controls  5   	   'vmclr-namespace:Export.ViewModel  5   	   .	converterclr-namespace:Export.Converter  5	   	   2bh)clr-namespace:Core.Behavior;assembly=Core  5
   	   XiPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity  5   	   $viewclr-namespace:Export.View  5   	    localclr-namespace:Export  5   	   -   5        $��_this��5   {   $	��450��6%   $	��800��62     � WindowStartupLocation$  CenterScreen=�6>    � WindowState$ 	Maximized=�6c   (  "MahApps.Metro.Controls.MetroWindow   TitleCharacterCasing$ Normal=�6�    � Title 6	   �� *	MainTitle+ � DataContext 5      *  $Export.ViewModel.MainWindowViewModel  5   
    � 	Resources 5      3  -Export.Converter.BooleanToVisibilityConverter    BooleanToVisibilityConverter0  *Export.Converter.ModeToVisibilityConverter  ModeToVisibilityConverter �� DataType$  Export.ViewModel.LiveViewModel  Export.View.LiveView&   Export.ViewModel.SearchViewModel  Export.View.SearchView  	textblock  checkbox d� 	IsChecked  cameraTemplate ��Name	 �� RelativeSource
 �� Path �� Triggers �� Value �� Binding �� ElementName �� UpdateSourceTrigger �� 
TargetName �� Value �� Property  siteTemplate �� ItemTemplate �� ItemsSource��%c  &	        &	      (�� &     5      *, +)(�� X     5      *, +)&	 �     &	 �     &	 1    7  7  &	 �    7    5   
   61     5   
   6.   �� 5   
   "  6   .R�5        5      �� 5   
   "  6   .R�5        5      �� 5   
   .R�5      �� ��6   �� *
CameraName+5       �� 5"   
   .R�5#      �� ��6   �� *
CameraName+ 66   �� *
IsSelected+5$      �� 5&   
   .R�5'      �� $ PeoplePresenter��6   8��  ��5(      �� 	 �� *TemplatedParent+$
 Content�5)       5*      �� 5+      $
 true��6�    6   �� $ _this��$
 DataContext.IsLiveAlarm�$ PropertyChanged=�.a�5,      �� $ PeoplePresenter��6   8   � ��5.      5/      51      �� 53   
   8    68   �� *CameraZones+.R�54      �� ��6   �� *
SiteName+55      56      .��57      � .+�58   
   �  � ColumnDefinitions 59      �� 5:      $	��290 �6#   �� 5;       � RowDefinitions 5=      �� 5>      $��35 �6    �� 5?      $��* �6    �� 5@      $��45 �6    .+�5B      ]� $��2��6   .+�5C      `� $	��Top=�6   .�5D      c� .@�5E      �� $	��130��6!   ��6-   �� *LiveCommand+��5F      �� 5G   "    �� 
TargetType" ��6(   .�5H   &   ��  � ��
 ���� 6C    �� Triggers 5I   &   �� 5J   *   	  Export.ViewModel.Mode 	  Live# Z 6V    66   �� *CurrentMode+.a�5K   .   ��  � �� �#5765736K   5L   +   5N   #   .��5P      ��  �� Orientation$ 
Horizontal=�6)   .+�5Q   "   �� $��Image/growth.png��6(   $��20��6B   �� 5R   "   $��
Live Alarn��6,   $��5,5,0,0q�6>   $��Black"�6O   
��� ���6b   5S      5T      �� 5U      $	��100��6!   ��6-   �� *HistoryCommand+��5V      �� 5W   "   " ��6(   .�5X   &   ��  � ��
 ���� 6C    5Y   &   �� 5Z   *    	  Search# Z 6V    66   �� *CurrentMode+.a�5[   .   ��  � �� �#5765736K   5\   +   5^   #   .��5`      �� $ 
Horizontal=�6)   .+�5a   "   �� $��Image/history.png��6(   $��20��6C   �� 5b   "   $��History��6,   $��Black"�6;   $��5,5,0,0q�6N   
��� ���6_   5c      5e      5f      5g      �� 5h       �� AcceptsReturn . 6   5i      � 5j      $��1��6    5k      �� 5l      $
��auto �6$   �� 5m      $
��auto �6$   �� 5n      $
��auto �6$   �� 5o      $
��auto �6$   �� 5p      $��* �6$   .+�5r      �� $��5q�6   $ 
Horizontal=�6�   $
��Left=�6�   $��Center=�6�   y�6(   �� *CurrentMode+ �� 	Converter# [ .+�5s      �� $��
Start Date��6    $��80��62   +
  %MahApps.Metro.Controls.DateTimePicker
  5t      $	��200��63   +  %MahApps.Metro.Controls.TimePickerBase   SelectedTimeFormat$
 Long=�5u       
  SelectedDateFormat$ Short=�6/     
  DisplayDateStart$  1/1/2010X�6L   ! 
  DisplayDateEnd$! 
12/31/2050X�6h   " 
  SelectedDate" 5t   ?   �� *	StartDate+# �� StringFormat$# MM/dd/yyyy HH:mm:ss��5u   h   5v      �� 5x      $��5q�6   $��1��6(   $ 
Horizontal=�6�   $
��Left=�6�   $��Center=�6�   y�65   �� *CurrentMode+# [ .+�5y      �� $��End Date��6    $��80��61   
  5z      $	��200��63   $  1/1/2010X�6`   $! 
12/31/2050X�6|   " 6?   �� *	EndDate+6|   5{      �� 5|      $��2��6   $��
Live alarm��6)   $
��Left=�6�   $��85,5,0,5q�6�    6>   �� *IsLiveAlarm+y�6`   �� *CurrentMode+# [ 6�   �� 5}      $��3��6   $��5q�6(   $��Option��63   ��5~      �� 5      " ��6      MetroGroupBox$ �� BasedOn#$ [ 66   .�5�      ��  � y�$ Visible=�6;   �� 5�      +  %MahApps.Metro.Controls.ControlsHelper �@ ContentCharacterCasing$ Normal=�6d    5�      �� 5�   "   # Z 6N    6.   �� *CurrentMode+.a�5�   &   ��  � y�$ 	Collapsed=�6C   5�   #   �� 5�   "    6.   �� *IsLiveAlarm+ true6N   .a�5�   &   ��  � y�$ 	Collapsed=�6C   5�   #   5�      .��5�      � .+�5�      	� $��
By minutes��6&   $��5q�6Z    6;   �� *
IsMinute+6g   	� 5�      $��By hours��6&   $��5q�6W    69   �� *	IsHours+6d   	� 5�      $��By days��6&   $��5q�6T    68   �� *IsDay+6a   5�      5�      �� 5�      $��Camera list��6   $��4��60   $��5,0,5,0q�6=   %   ContentCharacterCasing$% Normal=�6N   .��5�      R� #��[ 6   ��6L   �� *SiteCameras+.  (System.Windows.Interactivity.Interaction&   	Behaviors& 5�      0   *Core.Behavior.BindableSelectedItemBehavior  5�      '   ExpandSelected' . (   SelectedItem( 5�   .   �� *SelectedTree+) �� Mode$) TwoWay=�$ PropertyChanged=���5�      �� 5�      " Q�6$     MetroTreeViewItem#$ [ 6>   .�5�   "   ��  �@Q�
IsExpanded . 6?   5�      5�      5�      5�      �� 5�      $��1��6   $��1��6M   ��6-   �� *CurrentItem+6M   �� 5�      $��2��6n   $��5,0,5,5q�6{   $��30��6�   $	��100��6�   $��Search��6�   $��Right=�6�     customButton#��[ 6�   y�6   �� *CurrentMode+# [ ��6�   �� *SearchCommand+6�   �� 5�      $y�	Collapsed=�6   $��1��6,   $��5,0,5,5q�69   $��1��6J   $��30��6Z   $	��100��6f   $��Export��6r   $��Right=�6�   ��6�   �� *ExportCommand+5�      � 5�   
   
�������* J� Opacity$	* 0.4Q�6#   y�62   �� *	IsLoading+# [  �� 5�   
   
�������$	��100��6~   + �� CornerRadius$+ 5��6�   $��Center=�6�   $��Center=�6�   $��87��6�   y�6   �� *	IsLoading+# [  ._�5�      �� $��91��6   .+�5�      )  #MahApps.Metro.Controls.ProgressRing  $��40��6.   �� 5�      $��Processing ...��6   $��Center=�62   
���   �6O   5�      5�      5�      5�      