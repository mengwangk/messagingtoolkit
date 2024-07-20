package com.lightspeedleader.browser;

public class OptionObj
{

    public String name;
    public String value;
    public boolean flag;

    public OptionObj(String s, boolean flag1)
    {
        name = s;
        value = "";
        flag = flag1;
    }

    public void setValue(String s)
    {
        value = s;
    }
}
