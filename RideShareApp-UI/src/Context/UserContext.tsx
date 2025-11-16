// UserContext.tsx
import { createContext, useState, useContext, type ReactNode } from "react";

type User = { 
    firstName: string,
    lastName: string,
    email: string,
    phone: string,
    token: string,
} | null;

interface UserContextType {
    user: User;
    setUser: (user: User) => void;
}

// Create context with default empty value
const UserContext = createContext<UserContextType | undefined>(undefined);

// Provider component
export function UserProvider({ children }: { children: ReactNode }) {
    const [user, setUser] = useState<User>(null);
    return (
        <UserContext.Provider value={{ user, setUser }}>
            {children}
        </UserContext.Provider>
    );
}

// Custom hook for easy usage
export function useUser() {
    const context = useContext(UserContext);
    if (!context) {
        throw new Error("useUser must be used within a UserProvider");
    }
    return context;
}
