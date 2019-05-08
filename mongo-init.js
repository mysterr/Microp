db.createUser(
        {
            user: "demo",
            pwd: "demo123",
            roles: [
                {
                    role: "readWrite",
                    db: "productsdb"
                }
            ]
        }
);