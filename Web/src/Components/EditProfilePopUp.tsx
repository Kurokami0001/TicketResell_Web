import React, {
  ChangeEvent,
  Dispatch,
  FormEvent,
  SetStateAction,
  useState,
} from "react";
import { X, Eye, EyeOff, Loader2 } from "lucide-react";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/Components/ui/dialog";
import { Button } from "@/Components/ui/button";
import { Input } from "@/Components/ui/input";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/Components/ui/select";
import { useToast } from "@/Hooks/use-toast";
interface FormData {
  fullName: string | undefined;
  sex: string | undefined;
  phone: string | undefined;
  address: string | undefined;
  birthday: string | undefined;
  bio: string | undefined;
}

interface EditProfilePopupProps {
  isOpen: boolean;
  onClose: () => void;
  initialData?: FormData; // Chỉ định initialData là tùy chọn
  userId: string;
}

interface PasswordChangeProps {
  isOpen: boolean;
  setIsOpen: Dispatch<SetStateAction<boolean>>;
  userId: string;
  initialData?: FormData; // Chỉ định initialData là tùy chọn
}

interface Passwords {
  current: string;
  new: string;
  confirm: string;
}

interface PasswordErrors {
  current?: string;
  new?: string;
  confirm?: string;
}

interface Errors {
  [key: string]: string; // Định nghĩa rằng key có thể là bất kỳ chuỗi nào và value là một chuỗi
}

// API function for updating user profile
const updateUserProfile = async (userId: string, data: any) => {
  try {
    const response = await fetch(
      `http://localhost:5296/api/User/update/${userId}`,
      {
        method: "PUT",

        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(data),
      }
    );

    if (!response.ok) {
      const errorData = await response.json();
      throw new Error(errorData.message || "Failed to update profile");
    }

    return await response.json();
  } catch (error) {
    throw error;
  }
};

// Password validation
const validatePassword = (password: string) => {
  const minLength = 8;
  const hasUpperCase = /[A-Z]/.test(password);
  const hasLowerCase = /[a-z]/.test(password);
  const hasNumbers = /\d/.test(password);
  const hasSpecialChar = /[!@#$%^&*(),.?":{}|<>]/.test(password);

  const errors = [];
  if (password.length < minLength)
    errors.push(`Password must be at least ${minLength} characters long`);
  if (!hasUpperCase)
    errors.push("Password must contain at least one uppercase letter");
  if (!hasLowerCase)
    errors.push("Password must contain at least one lowercase letter");
  if (!hasNumbers) errors.push("Password must contain at least one number");
  if (!hasSpecialChar)
    errors.push("Password must contain at least one special character");

  return errors;
};

// Form validation
const validateForm = (formData: FormData) => {
  const errors: { [key: string]: string } = {};

  if (!formData.fullName?.trim()) errors.fullName = "Full name is required";
  if (!formData.sex) errors.sex = "Sex is required";
  if (!formData.phone?.trim()) errors.phone = "Phone number is required";
  else if (!/^\+?\d{10,}$/.test(formData.phone?.trim()))
    errors.phone = "Invalid phone number format";
  if (!formData.address?.trim()) errors.address = "Address is required";
  if (!formData.birthday) errors.birthday = "Birthday is required";

  return errors;
};

// Password Change Component
// Password Change Component
const PasswordChange = ({ isOpen, setIsOpen, userId }: PasswordChangeProps) => {
  const { toast } = useToast(); // Use the Shadcn toast hook
  const [showPassword, setShowPassword] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [errors, setErrors] = useState<PasswordErrors>({});
  const [passwords, setPasswords] = useState<Passwords>({
    current: "",
    new: "",
    confirm: "",
  });
  const handleChangePassword = async (userId: string, data: any) => {
    if (passwords.new !== passwords.confirm) {
      alert("New passwords do not match!");
      return;
    }

    try {
      const response = await fetch(
        `http://localhost:5296/api/Authentication/change-password`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify({
            UserId: userId,
            CurrentPassword: passwords.current,
            NewPassword: passwords.new,
          }),
        }
      );

      if (response.ok) {
        alert("Password changed successfully!");
        setPasswords({
          current: "",
          new: "",
          confirm: "",
        });
      } else {
        const errorData = await response.json();
        alert(`Error: ${errorData.title || "Error changing password"}`);
      }
    } catch (error) {
      console.error("Error changing password:", error);
      alert("An error occurred while changing the password.");
    }
  };

  const handlePasswordChange = (e: ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;

    setPasswords({
      ...passwords,
      [name]: value,
    });
    // Clear errors when user types
    if (errors[name as keyof PasswordErrors]) {
      setErrors({
        ...errors,
        [name]: "",
      });
    }
  };

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setErrors({});

    // Validate passwords
    const newPasswordErrors = validatePassword(passwords.new);
    if (newPasswordErrors.length > 0) {
      setErrors({ new: newPasswordErrors.join(". ") });
      return;
    }

    if (passwords.new !== passwords.confirm) {
      setErrors({ confirm: "Passwords don't match" });
      return;
    }

    try {
      setIsLoading(true);
      // Add your password update API call here
      await handleChangePassword(userId, {
        currentPassword: passwords.current,
        newPassword: passwords.new,
      });

      // Using the Shadcn toast method
      toast({
        title: "Success",
        description: "Password updated successfully.",
        variant: "default", // Variant for success message
        duration: 5000, // Duration in milliseconds (optional)
      });
      setIsOpen(false);
    } catch (error) {
      const errorMessage =
        error instanceof Error ? error.message : "Failed to update password";

      // Using the Shadcn toast method for errors
      toast({
        title: "Error",
        description: errorMessage,
        variant: "destructive", // Indicating an error variant
        duration: 5000, // Duration in milliseconds (optional)
      });
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <Dialog open={isOpen} onOpenChange={setIsOpen}>
      <DialogContent className="bg-white rounded-t-xl sm:rounded-xl w-full max-w-md">
        <DialogHeader>
          <DialogTitle>Change Password</DialogTitle>
        </DialogHeader>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="space-y-1">
            <div className="relative">
              <Input
                type={showPassword ? "text" : "password"}
                name="current"
                placeholder="Current Password"
                value={passwords.current}
                onChange={handlePasswordChange}
                className={`pr-10 ${errors.current ? "border-red-500" : ""}`}
                disabled={isLoading}
              />
              <button
                type="button"
                onClick={() => setShowPassword(!showPassword)}
                className="absolute right-3 top-1/2 transform -translate-y-1/2"
                disabled={isLoading}
              ></button>
            </div>
            {errors.current && (
              <p className="text-sm text-red-500">{errors.current}</p>
            )}
          </div>

          <div className="space-y-1">
            <Input
              type="password"
              name="new"
              placeholder="New Password"
              value={passwords.new}
              onChange={handlePasswordChange}
              className={errors.new ? "border-red-500" : ""}
              disabled={isLoading}
            />
            {errors.new && <p className="text-sm text-red-500">{errors.new}</p>}
          </div>

          <div className="space-y-1">
            <Input
              type="password"
              name="confirm"
              placeholder="Confirm New Password"
              value={passwords.confirm}
              onChange={handlePasswordChange}
              className={errors.confirm ? "border-red-500" : ""}
              disabled={isLoading}
            />
            {errors.confirm && (
              <p className="text-sm text-red-500">{errors.confirm}</p>
            )}
          </div>

          <div className="flex justify-end space-x-2">
            <Button
              className="hover:bg-gray-200 rounded"
              variant="outline"
              onClick={() => setIsOpen(false)}
              disabled={isLoading}
            >
              Cancel
            </Button>
            <Button
              className="bg-green-500 text-white hover:bg-green-400 rounded"
              type="submit"
              disabled={isLoading}
            >
              {isLoading ? (
                <>
                  <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                  Updating
                </>
              ) : (
                "Update Password"
              )}
            </Button>
          </div>
        </form>
      </DialogContent>
    </Dialog>
  );
};

// Main Profile Edit Component
const EditProfilePopup: React.FC<EditProfilePopupProps> = ({
  isOpen,
  onClose,
  initialData,
  userId,
}) => {
  const [showPasswordDialog, setShowPasswordDialog] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [errors, setErrors] = useState<Errors>({});
  const { toast } = useToast();
  const HandleSubmitClick = () => {
    window.location.reload();
  };
  const [formData, setFormData] = useState<FormData>(
    initialData || {
      fullName: "",
      sex: "",
      phone: "",
      address: "",
      birthday: "",
      bio: "",
    }
  );

  // Function to validate if the age is 18 or over
  const validateBirthday = (birthday: string) => {
    const selectedDate = new Date(birthday);
    const today = new Date();

    // Calculate age by subtracting birth year from current year
    const age = today.getFullYear() - selectedDate.getFullYear();

    // Adjust age if the birthday hasn't occurred yet this year
    const hasHadBirthdayThisYear =
      today.getMonth() > selectedDate.getMonth() ||
      (today.getMonth() === selectedDate.getMonth() &&
        today.getDate() >= selectedDate.getDate());

    const finalAge = hasHadBirthdayThisYear ? age : age - 1;

    if (finalAge < 18) {
      setErrors((prevErrors) => ({
        ...prevErrors,
        birthday: "You must be at least 18 years old.",
      }));
    } else {
      setErrors((prevErrors) => ({
        ...prevErrors,
        birthday: "",
      }));
    }
  };
  const handleInputChange = (e: ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;

    setFormData({
      ...formData,
      [name]: value,
    });
    // Clear errors when user types
    if (errors[name as keyof Errors]) {
      setErrors({
        ...errors,
        [name]: "",
      });
    }
    // Check if the input name is birthday to perform age validation
    if (name === "birthday") {
      validateBirthday(value); // Call validation function
    }
  };

  const handleSexChange = (value: string) => {
    setFormData({
      ...formData,
      sex: value,
    });
    if (errors.sex) {
      setErrors({
        ...errors,
        sex: "",
      });
    }
  };

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    // Validate form
    const validationErrors = validateForm(formData);
    if (Object.keys(validationErrors).length > 0) {
      setErrors(validationErrors);
      return;
    }

    try {
      setIsLoading(true);
      await updateUserProfile(userId, formData);

      toast({
        title: "Success",
        description: "Profile updated successfully.",
        variant: "default", // Using unified type
        duration: 5000, // Optional duration
      });
      onClose();
    } catch (error: any) {
      toast({
        title: "Error",
        description: error.message || "Failed to update profile.",
        variant: "destructive",
        duration: 5000,
      });
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <>
      <Dialog open={isOpen} onOpenChange={onClose}>
        <DialogContent className="bg-white rounded-t-xl sm:rounded-xl w-full max-w-2xl">
          <DialogHeader>
            <DialogTitle>Edit Profile</DialogTitle>
          </DialogHeader>
          <form onSubmit={handleSubmit} className="space-y-4">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div className="space-y-2">
                <label htmlFor="fullName" className="text-sm font-medium">
                  Full Name
                </label>
                <Input
                  id="fullName"
                  name="fullName"
                  value={formData.fullName}
                  onChange={handleInputChange}
                  placeholder="John Doe"
                  className={errors.fullName ? "border-red-500" : ""}
                  disabled={isLoading}
                />
                {errors.fullName && (
                  <p className="text-sm text-red-500">{errors.fullName}</p>
                )}
              </div>

              <div className="space-y-2 ">
                <label htmlFor="sex" className=" text-sm font-medium">
                  Sex
                </label>
                <div className="hover:bg-gray-200">
                  <Select
                    value={formData.sex}
                    onValueChange={handleSexChange}
                    disabled={isLoading}
                  >
                    <SelectTrigger
                      className={errors.sex ? "border-red-500" : ""}
                    >
                      <SelectValue placeholder="Select gender" />
                    </SelectTrigger>
                    <SelectContent className="bg-white">
                      <SelectItem value="male">Male</SelectItem>
                      <SelectItem value="female">Female</SelectItem>
                      <SelectItem value="other">Other</SelectItem>
                    </SelectContent>
                  </Select>
                </div>
                {errors.sex && (
                  <p className="text-sm text-red-500">{errors.sex}</p>
                )}
              </div>

              <div className="space-y-2">
                <label htmlFor="phone" className="text-sm font-medium">
                  Phone
                </label>
                <Input
                  id="phone"
                  name="phone"
                  value={formData.phone}
                  onChange={handleInputChange}
                  placeholder="+1 234 567 8900"
                  className={errors.phone ? "border-red-500" : ""}
                  disabled={isLoading}
                />
                {errors.phone && (
                  <p className="text-sm text-red-500">{errors.phone}</p>
                )}
              </div>

              <div className="space-y-2">
                <label htmlFor="birthday" className="text-sm font-medium">
                  Birthday
                </label>
                <Input
                  id="birthday"
                  name="birthday"
                  type="date"
                  value={
                    formData.birthday
                      ? new Date(formData.birthday).toISOString().split("T")[0]
                      : "No birthday provided"
                  }
                  onChange={handleInputChange}
                  className={errors.birthday ? "border-red-500" : ""}
                  disabled={isLoading}
                />
                {errors.birthday && (
                  <p className="text-sm text-red-500">{errors.birthday}</p>
                )}
              </div>

              <div className="space-y-2 md:col-span-2">
                <label htmlFor="address" className="text-sm font-medium">
                  Address
                </label>
                <Input
                  id="address"
                  name="address"
                  value={formData.address}
                  onChange={handleInputChange}
                  placeholder="123 Street Name, City, Country"
                  className={errors.address ? "border-red-500" : ""}
                  disabled={isLoading}
                />
                {errors.address && (
                  <p className="text-sm text-red-500">{errors.address}</p>
                )}
              </div>
            </div>

            <div className="flex justify-between items-center pt-4">
              <Button
                className="hover:bg-gray-200 rounded"
                type="button"
                variant="outline"
                onClick={() => setShowPasswordDialog(true)}
                disabled={isLoading}
              >
                Change Password
              </Button>
              <div className="space-x-2">
                <Button
                  className="hover:bg-gray-200 rounded"
                  variant="outline"
                  onClick={onClose}
                  disabled={isLoading}
                >
                  Cancel
                </Button>
                <Button
                  className="bg-green-500 text-white hover:bg-green-400 rounded"
                  onClick={HandleSubmitClick}
                  type="submit"
                  disabled={isLoading}
                >
                  {isLoading ? (
                    <>
                      <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                      Saving
                    </>
                  ) : (
                    "Save Changes"
                  )}
                </Button>
              </div>
            </div>
          </form>
        </DialogContent>
      </Dialog>

      <PasswordChange
        isOpen={showPasswordDialog}
        setIsOpen={setShowPasswordDialog}
        userId={userId}
        initialData={initialData}
      />
    </>
  );
};

export default EditProfilePopup;
