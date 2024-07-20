package com.lightspeedleader.browser;

public class HuffmanTable {

    Node node;
    public static int deCode_code = 0;

    HuffmanTable() {
        node = new Node();
    }

    public boolean addNode(int i, int j) {
        return addNode(node, i, j);
    }

    public boolean addNode(Node node, int i, int j) {
        if (i == 1) {
            if (node._fld012B == null) {
                node._fld012B = new Node(j);
                return true;
            }
            if (node._fld012C == null) {
                node._fld012C = new Node(j);
                return true;
            } else {
                return false;
            }
        }
        if (node._fld012B == null) {
            node._fld012B = new Node();
            return addNode(node._fld012B, i - 1, j);
        }
        if (!node._fld012B._fld012D && addNode(node._fld012B, i - 1, j)) {
            return true;
        }
        if (node._fld012C == null) {
            node._fld012C = new Node();
            return addNode(node._fld012C, i - 1, j);
        }
        if (!node._fld012C._fld012D) {
            return addNode(node._fld012C, i - 1, j);
        } else {
            return false;
        }
    }

    public void clear() {
        if (node._fld012B != null) {
            clearNode(node._fld012B);
        }
        if (node._fld012C != null) {
            clearNode(node._fld012C);
        }
        node._fld012B = null;
        node._fld012C = null;
        node = null;
    }

    public void clearNode(Node node) {
        if (node._fld012B != null) {
            clearNode(node._fld012B);
        }
        if (node._fld012C != null) {
            clearNode(node._fld012C);
        }
        node = null;
    }

    public boolean deCode(DecodeParam decodeParam) {
        boolean flag = false;
        Node node = this.node;
        int i = decodeParam.DD;
        int j = decodeParam.DD;
        int k = decodeParam.DE;
        int l = decodeParam.DC[i];
        i++;
        if (l == 255) {
            for (; decodeParam.DC[i] == 255; i++) {
            }
            if (decodeParam.DC[i] != 0) {
                return false;
            }
            i++;
        }
        l <<= k;
        do {
            do {
                boolean flag1 = false;
                if ((l & 0x80) != 0) {
                    node = node._fld012C;
                } else {
                    node = node._fld012B;
                }
                k++;
                if (node == null) {
                    node = this.node;
                    k = 8;
                    flag1 = true;
                }
                if (!flag1 && node._fld012D) {
                    decodeParam.DE = k;
                    if (k == 8) {
                        decodeParam.DD = i;
                        decodeParam.DE = 0;
                    }
                    decodeParam.DF -= decodeParam.DD - j;
                    deCode_code = node._fld012E;
                    return true;
                }
                if (k != 8) {
                    break;
                }
                k = 0;
                decodeParam.DD = i;
                l = decodeParam.DC[i];
                i++;
                if (l == 255) {
                    for (; decodeParam.DC[i] == 255; i++) {
                    }
                    if (decodeParam.DC[i] != 0) {
                        decodeParam.DF -= decodeParam.DD - j;
                        return false;
                    }
                    i++;
                }
            } while (true);
            l <<= 1;
        } while (l != 0x75bcd15);
        return true;
    }

}
