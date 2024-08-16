table 50100 "My Table"
{
    fields
    {
        field(1; "My Field"; Integer)
        {
            Caption = 'My Field';
        }

        field(2; "My Other Field"; Integer)
        {
            Caption = 'My Other Field', Locked = true, MaxLength = 30;
        }

        field(3; "Already Translated Field"; Integer)
        {
            Caption = 'Already Translated Field', Comment = 'nl-NL = Reeds vertaald veld';
        }
    }
}