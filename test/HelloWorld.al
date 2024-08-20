// Welcome to your new AL extension.
// Remember that object names and IDs should be unique across all extensions.
// AL snippets start with t*, like tpageext - give them a try and happy coding!

namespace DefaultPublisher.test;

using Microsoft.Sales.Customer;

pageextension 50100 CustomerListExt extends "Customer List"
{
    layout
    {
        addafter("Allow Multiple Posting Groups")
        {
            field("Contact ID"; Rec."Contact ID")
            {
                ApplicationArea = All;
                Caption = 'Added Control';
            }
        }

        modify("Application Method")
        {
            Caption = 'Changed Control Caption';
        }
    }

    actions
    {
        addafter("&Customer")
        {
            action(Foo)
            {
                Caption = 'Added Action';
                ApplicationArea = All;
                Image = "8ball";
            }
        }

        modify(ApplyTemplate)
        {
            Caption = 'Changed Action Caption';
        }
    }
}