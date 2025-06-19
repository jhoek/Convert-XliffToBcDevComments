table 50100 MyTable
{
    fields
    {
        field(1; "No Dev Comments"; Integer)
        {
            Caption = 'No Dev Comments'; // should complain about missing comments
        }

        field(2; "Some Language Missing"; Integer)
        {
            Caption = 'Some Language Missing', Comment = 'nl-NL=Sommige ontbrekende talen'; // should complain about missing fr-FR translation
        }

        field(3; "All Languages Present"; Integer)
        {
            Caption = 'All Languages Present', Comment = 'nl-NL=Alle talen aanwezig|fr-FR=Alle talen aanwezig in het Frans ;)'; // should not complain
        }

        field(4; "Locked"; Integer)
        {
            Caption = 'Locked', Locked = true; // should not complain
        }

        field(5; "Wrong separator"; Integer)
        {
            Caption = 'Wrong separator', Comment = 'nl-NL=Verkeerde scheider$fr-FR=Verkeerde scheider in het Frans ;)'; // should complain about missing fr-FR translation
        }

        field(6; "Too Long"; Integer)
        {
            Caption = 'Too Long', Comment = 'nl-NL=1234567890|fr-FR=1234567890', MaxLength = 8; // should complain about both translations being too long
        }
    }
}