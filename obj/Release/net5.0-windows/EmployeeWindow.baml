   M S B A M L   `   `   `  ���� C  ?Teretana, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:TeretanaTeretana  7 clr-namespace:Teretana.ResourcesTeretana.Resources      Teretana.EmployeeWindow   W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     (localclr-namespace:Teretana     /rs clr-namespace:Teretana.Resources       � Title$  EmployeeWindow��$	��650��$
��1000�� � 	Resources  �� MergedDictionaries �� Source��  �� $ /Styles.xaml?�%    .��� ����  �� ImageSource$ /assets/background.png��.+��� -    ��Name$ settingsBtn��   GreenBtnStyle#��[  $
��Left=�$��
943,10,0,0q�$	��Top=�.���� $��assets/settings.png��$��19��$��25���� -    ��Name$ membersListView��
���   �$��2q�$��10,78,790,78q� �� View �� .P��� $��Id�� �� DisplayMemberBinding �� *Id+�� "   Teretana.Resources.Resources	   Name_lbl#��Z	  �� *Name+�� 
   Surname#��Z
  �� *	Surname+��    Birth_date_lbl#��Z  �� *
DateTime+�� 
���   �$��2q�   Member_header#��Z $��215,71,10,78q�.��� $��
12,0,-12,0q�.+��� 
���   �$��2q�   Info_header#��Z $��0,177,435,5q�.��� $��	0,30,0,-5q�$	��220��.+��� #��Z	 $
��Left=�$��10,0,0,0q�$	��Top=��� -    ��Name$ nameTextBox��$
��Left=�$��	116,1,0,0q� �� TextWrapping$
 Wrap=�$	��Top=�$	��120��$��24���� #��Z
 $
��Left=�$��	10,31,0,0q�$	��Top=��� -   $ surnameTextBox��$
��Left=�$��
116,32,0,0q�$
 Wrap=�$	��Top=�$	��120��$��24���� #��Z $
��Left=�$��	10,61,0,0q�$	��Top=� J� RenderTransformOrigin$ 0.718,-0.283&�(  "System.Windows.Controls.DatePicker  -     Name$ 
datePicker��$
��Left=�$��
116,61,0,0q�$	��Top=�$	��120��$��26����    Jmbg_lbl#��Z $
��Left=�$��	10,92,0,0q�$	��Top=��� -   $ jmbgTextBox��$
��Left=�$��
116,93,0,0q�$
 Wrap=�$	��Top=�$	��120��$��24����    City_lbl#��Z $
��Left=�$��
10,123,0,0q�$	��Top=��� -    ��Name$ citiesComboBox��$
��Left=�$��116,125,0,0q�$	��Top=�$	��120��$��26����    	Email_lbl#��Z $
��Left=�$��
10,154,0,0q�$	��Top=��� -   $ emailTextBox��$
��Left=�$��116,156,0,0q�$
 Wrap=�$	��Top=�$	��120��$��24���� 
���   �$��2q�   Trainings_header#��Z $��382,177,15,5q�.��� $	��237��.+��� -	   $ trainingsListView��$��0,0,0,39q� �� .P���    Date_header#��Z  �� Width$	 100�� �� *Date+��    From_header#��Z $	 100�� �� *From+��    Until_header#��Z $	 100�� �� *Until+�� -
   $ startTrainingBtn��#��[     	Start_btn#��Z $
��Left=�$��118,201,0,0q�$	��Top=�$��54���� -   $ stopTrainingBtn��#��[     Stop_btn#��Z $
��Left=�$��177,201,0,0q�$	��Top=�$��54����    	Start_lbl#��Z $
��Left=�$��	0,198,0,0q�$	��Top=��� 
���   �$��2q�   Membership_header#��Z $��382,6,15,285q�.��� $��0,0,8,0q�.+���    Paid_lbl#��Z $
��Left=�$	��Top=�$��	17,18,0,0q���     	Valid_lbl#��Z  $
��Left=�$��	17,44,0,0q�$	��Top=��� !   Days_lbl#��Z! $
��Left=�$��	17,74,0,0q�$	��Top=��� -   " ��Name$" 	paidAtLbl��$
��Left=�$��
104,18,0,0q�$	��Top=��� -   $" validUntilLbl��$
��Left=�$��
104,44,0,0q�$	��Top=��� -   $" daysLeftLbl��$
��Left=�$��
104,74,0,0q�$	��Top=��� $��237,10,0,19q�.��� $	��105��.+��� -   $ newMembershipBtn��#��[  #   New_btn#��Z# $��Center=�$	��Top=�$��54��$��0,10,0,0q��� -   $ extendMembershipBtn��#��[  $   Edit_btn#��Z$ $��Center=�$	��Top=�$��54��$��0,37,0,0q��� -   $ 	revokeBtn��#��[  %   
Revoke_btn#��Z% $��Center=�$	��Top=�$��54��$��0,64,0,0q�$ 1.037,0.561&��� -   & ��Name$& avatarImage��$
��Left=�$	��147��$��	90,10,0,0q�$	��Top=�$	��160��$��/avatar.png���� -   $ 	avatarBtn��#��[  $
��Left=�$��237,139,0,0q�$	��Top=�$
��Bold"�$��15(�$��23��$��27��.���� $��assets/person.png��$��13��$��17���� '   Members_lbl#��Z' $
��Left=�$��	10,50,0,0q�$	��Top=��� -   $ addBtn��#��[  (   Add_btn#��Z( $
��Left=�$��
215,46,0,0q�$	��Top=�$��54���� -   $ editBtn��#��[  #��Z$ $
��Left=�$��
274,46,0,0q�$	��Top=�$��54���� -   $ 	deleteBtn��#��[  )   
Delete_btn#��Z) $
��Left=�$��
333,46,0,0q�$	��Top=�$��54���� -   $ 	logoutBtn��*   
Logout_btn#��Z* #��[  $
��Left=�$��
920,46,0,0q�$	��Top=�$��54��$��25���� -   $ saveBtn��#��[  +   Save_btn#��Z+ $
��Left=�$��
392,46,0,0q�$	��Top=�$��54��$��22��