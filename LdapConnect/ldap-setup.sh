#!/bin/bash

# Install packages
sudo apt update
sudo DEBIAN_FRONTEND=noninteractive apt install -y slapd ldap-utils

# Set variables
LDAP_DOMAIN="example.com"
LDAP_DC="dc=example,dc=com"
ADMIN_PASS="admin"

# Configure slapd
sudo debconf-set-selections <<EOF
slapd slapd/internal/generated_adminpw password ${ADMIN_PASS}
slapd slapd/internal/adminpw password ${ADMIN_PASS}
slapd slapd/password2 password ${ADMIN_PASS}
slapd slapd/password1 password ${ADMIN_PASS}
slapd slapd/domain string ${LDAP_DOMAIN}
slapd slapd/purge_database boolean true
slapd shared/organization string ${LDAP_DOMAIN}
slapd slapd/backend select MDB
slapd slapd/move_old_database boolean true
EOF

sudo dpkg-reconfigure -f noninteractive slapd

# Create users.ldif
cat <<EOF > /tmp/users.ldif
dn: ou=users,${LDAP_DC}
objectClass: organizationalUnit
ou: users

dn: uid=test1,ou=users,${LDAP_DC}
objectClass: top
objectClass: person
objectClass: organizationalPerson
objectClass: inetOrgPerson
cn: Test User1
sn: User1
uid: test1
userPassword: $(slappasswd -s "1234567890")

dn: uid=test2,ou=users,${LDAP_DC}
objectClass: top
objectClass: person
objectClass: organizationalPerson
objectClass: inetOrgPerson
cn: Test User2
sn: User2
uid: test2
userPassword: $(slappasswd -s "1234567890")
EOF

# Add users
ldapadd -x -D "cn=admin,${LDAP_DC}" -w ${ADMIN_PASS} -f /tmp/users.ldif

# Clean up
rm /tmp/users.ldif

echo "Setup complete. Test with: ldapsearch -x -H ldap://localhost -D uid=test1,ou=users,${LDAP_DC} -w 1234567890 -b ${LDAP_DC}"